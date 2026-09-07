const tokenKey = "busLinesToken";
const token = localStorage.getItem(tokenKey);
const authStatus = document.getElementById("auth-status");
const errorMessage = document.getElementById("chat-error");
const userChat = document.getElementById("user-chat");
const adminChat = document.getElementById("admin-chat");
const userMessages = document.getElementById("user-messages");
const userMessageForm = document.getElementById("user-message-form");
const userMessage = document.getElementById("user-message");
const adminMessages = document.getElementById("admin-messages");
const adminMessageForm = document.getElementById("admin-message-form");
const adminMessage = document.getElementById("admin-message");
const adminSendButton = adminMessageForm.querySelector("button");
const conversationList = document.getElementById("conversation-list");
const selectedUser = document.getElementById("selected-user");
const refreshConversations = document.getElementById("refresh-conversations");
let connection;
let currentUserId = null;
let selectedConversationId = null;

function decodeToken(jwt) {
  try {
    const encoded = jwt.split(".")[1];
    if (!encoded) return null;
    const padded = encoded.replace(/-/g, "+").replace(/_/g, "/") + "===";
    return JSON.parse(atob(padded.slice(0, padded.length - (padded.length % 4))));
  } catch { return null; }
}

function claim(payload, type, fallback) {
  return payload?.[type] ?? payload?.[fallback] ?? null;
}

function identity(payload) {
  return claim(payload, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "name")
    || payload?.unique_name
    || payload?.preferred_username
    || payload?.sub
    || null;
}

function role(payload) {
  return claim(payload, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "role");
}

function showError(message) { errorMessage.textContent = message; }

function appendMessage(container, item) {
  const loggedInUsername = identity(decodeToken(localStorage.getItem(tokenKey) || ""));
  const wrapper = document.createElement("div");
  wrapper.className = item.senderUsername === loggedInUsername ? "message message-sent" : "message message-received";
  wrapper.textContent = `${item.senderUsername === loggedInUsername ? "You" : item.senderUsername}: ${item.message}`;
  container.appendChild(wrapper);
  container.scrollTop = container.scrollHeight;
}

function renderMessages(container, messages) {
  container.replaceChildren();
  messages.forEach(item => appendMessage(container, item));
}

async function apiGet(url) {
  const response = await fetch(url, { headers: { Authorization: `Bearer ${localStorage.getItem(tokenKey) || ""}` } });
  if (!response.ok) {
    const error = new Error("Request failed.");
    error.status = response.status;
    throw error;
  }
  return response.json();
}

async function loadUserMessages() {
  renderMessages(userMessages, await apiGet("/api/chat/messages"));
}

async function loadConversations() {
  const conversations = await apiGet("/api/chat/conversations");
  conversationList.replaceChildren();
  conversations.forEach(conversation => {
    const button = document.createElement("button");
    button.type = "button";
    button.className = `conversation-item${conversation.id === selectedConversationId ? " selected" : ""}`;
    const name = document.createElement("strong");
    name.textContent = conversation.username;
    const last = document.createElement("span");
    last.textContent = conversation.lastMessage?.text || "No messages yet";
    button.append(name, last);
    button.addEventListener("click", () => openConversation(conversation));
    conversationList.appendChild(button);
  });
}

async function openConversation(conversation) {
  const previousConversationId = selectedConversationId;
  selectedConversationId = conversation.id;
  selectedUser.textContent = `Chat with ${conversation.username}`;
  renderMessages(adminMessages, await apiGet(`/api/chat/messages?conversationUserId=${conversation.id}`));
  adminMessage.disabled = false;
  adminSendButton.disabled = false;
  await connection.invoke("OpenConversation", conversation.id, previousConversationId);
  await loadConversations();
}

async function sendMessage(form, input, conversationId) {
  const message = input.value.trim();
  if (!message || !connection || connection.state !== signalR.HubConnectionState.Connected) return;
  try {
    await connection.invoke("SendMessage", message, conversationId);
    input.value = "";
  } catch (error) {
    showError(error.message || "Unable to send the message.");
  }
}

if (!token) {
  authStatus.textContent = "Log in from the main page to use chat.";
  userMessageForm.hidden = true;
} else {
  const payload = decodeToken(token);
  const username = identity(payload);
  const userRole = role(payload);
  authStatus.textContent = username ? `Signed in as ${username}.` : "Signed in.";

  if (userRole === "Admin") {
    adminChat.hidden = false;
  } else {
    userChat.hidden = false;
    currentUserId = Number(payload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || payload?.sub || 0);
  }

  connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub", { accessTokenFactory: () => localStorage.getItem(tokenKey) || "" })
    .withAutomaticReconnect()
    .build();

  connection.on("ReceiveMessage", message => {
    if (userRole === "Admin") {
      if (message.conversationUserId === selectedConversationId) appendMessage(adminMessages, message);
      loadConversations().catch(() => {});
    } else if (!currentUserId || message.conversationUserId === currentUserId) {
      appendMessage(userMessages, message);
    }
  });

  connection.on("ConversationUpdated", () => {
    if (userRole === "Admin") loadConversations().catch(() => {});
  });

  async function initializeChat() {
    try {
      // Verify the bearer token against the API first. This gives the user a
      // useful authentication error instead of masking every connection
      // failure as a bad session.
      await apiGet(userRole === "Admin" ? "/api/chat/conversations" : "/api/chat/messages");
      await connection.start();
      if (userRole === "Admin") await loadConversations();
      else await loadUserMessages();
    } catch (error) {
      console.error("Chat initialization failed", error);
      const status = error?.status;
      if (status === 401 || !localStorage.getItem(tokenKey)) {
        showError("Your session could not be verified. Please log in again.");
      } else {
        showError("Chat could not connect. Please refresh the page and try again.");
      }
    }
  }

  initializeChat();

  userMessageForm.addEventListener("submit", event => {
    event.preventDefault();
    sendMessage(userMessageForm, userMessage, null);
  });

  adminMessageForm.addEventListener("submit", event => {
    event.preventDefault();
    if (selectedConversationId) sendMessage(adminMessageForm, adminMessage, selectedConversationId);
  });

  refreshConversations.addEventListener("click", () => loadConversations().catch(() => showError("Could not refresh conversations.")));
}
