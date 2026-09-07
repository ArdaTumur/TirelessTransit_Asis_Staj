const form = document.querySelector("form");
const message = document.getElementById("form-message");
const isLogin = form.id === "login-form";
form.addEventListener("submit", async event => {
  event.preventDefault(); message.textContent = "";
  const button = form.querySelector("button"); const data = Object.fromEntries(new FormData(form)); button.disabled = true;
  try {
    const response = await fetch(isLogin ? "/api/login" : "/api/register", { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(data) });
    if (!response.ok) throw new Error(isLogin ? "Invalid username or password." : "Could not create the account. The username may already be in use.");
    if (isLogin) { localStorage.setItem("busLinesToken", await response.text()); window.location.assign("../"); return; }
    window.location.assign("../login/");
  } catch (error) { message.textContent = error.message; }
  finally { button.disabled = false; }
});
