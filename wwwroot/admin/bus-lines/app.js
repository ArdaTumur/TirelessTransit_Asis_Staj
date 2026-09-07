const apiUrl = "/api/bus-lines";
const form = document.getElementById("bus-line-form");
const formMessage = document.getElementById("form-message");
const loadingMessage = document.getElementById("loading-message");
const busLinesContainer = document.getElementById("bus-lines");
const submitButton = document.getElementById("submit-button");
const cancelEditButton = document.getElementById("cancel-edit-button");
const logoutButton = document.getElementById("logout-button");
let editingId = null;

const plateRegex = /^(0[1-9]|[1-7][0-9]|8[01])\s*(([A-Z])\s*(\d{4,5})|([A-Z]{2})\s*(\d{3,4})|([A-Z]{3})\s*(\d{2,3}))$/;
const driverRegex = /^[A-Za-zşçğüıöÖÇŞİĞÜ]+(?:[-'\s][A-Za-zşçğüıöÖÇŞİĞÜ]+)*$/;
const hoursRegex = /^([01]\d|2[0-3]):([0-5]\d)\s*-\s*([01]\d|2[0-3]):([0-5]\d)$/;

function formatTimeRange(input) {
  const flexibleRegex = /^([01]\d|2[0-3]):([0-5]\d)\s*-\s*([01]\d|2[0-3]):([0-5]\d)$/;
  if (!flexibleRegex.test(input.trim())) {
    return null;
  }
  return input.trim().replace(/\s*-\s*/, ' - ');
}

const getToken = () => localStorage.getItem("busLinesToken");

function getRoleFromToken(jwt) {
  try {
    const payload = JSON.parse(atob(jwt.split(".")[1].replace(/-/g, "+").replace(/_/g, "/")));
    return payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || payload.role || null;
  } catch { return null; }
}


function setMessage(message, isError = false) { 
  formMessage.textContent = message; 
  formMessage.classList.toggle("error", isError); 
}

function requestOptions(method, body) {
  const headers = { "Content-Type": "application/json" };
  if (getToken()) headers.Authorization = `Bearer ${getToken()}`; 
  return { method, headers, body: body ? JSON.stringify(body) : undefined };
}

function updateAuthUi() {
  const admin = getRoleFromToken(getToken() || "") === "Admin";
  document.getElementById("access-denied").hidden = admin;
  document.getElementById("admin-content").hidden = !admin;
  if (!admin) { setMessage("Admin access required.", true); return; }
  logoutButton.hidden = false;
  submitButton.disabled = false;
  form.querySelectorAll("input, select").forEach(input => input.disabled = false);
}

function resetForm() { 
  editingId = null; 
  form.reset(); 
  submitButton.textContent = "Add bus line"; 
  cancelEditButton.hidden = true; 
}

function createLineElement(busLine) {
  const item = document.createElement("article"); 
  item.className = "bus-line";
  const details = document.createElement("div");

  const plate = document.createElement("h3");
  plate.textContent = busLine.plate;

  const driver = document.createElement("p");
  driver.textContent = `Driver: ${busLine.driver}`;

  const hours = document.createElement("p");
  hours.textContent = `Hours: ${busLine.hours}`;

  const route = document.createElement("p");
  route.textContent = `Route: ${busLine.originCity} → ${busLine.destinationCity}`;

  const status = document.createElement("p");
  status.textContent = `Status: ${busLine.routeStatus}`;

  details.append(plate, driver, hours, route, status); 
  item.append(details);
  
  if (getRoleFromToken(getToken() || "") === "Admin") {
    const actions = document.createElement("div"); 
    actions.className = "line-actions";
    
    const editButton = document.createElement("button"); 
    editButton.className = "button secondary"; 
    editButton.textContent = "Edit"; 
    editButton.addEventListener("click", () => startEdit(busLine));
    
    const deleteButton = document.createElement("button"); 
    deleteButton.className = "button danger"; 
    deleteButton.textContent = "Delete"; 
    deleteButton.addEventListener("click", () => deleteBusLine(busLine.id));
    
    actions.append(editButton, deleteButton); 
    item.append(actions);
  }
  return item;
}

function displayBusLines(busLines) {
  busLinesContainer.replaceChildren();
  if (!busLines.length) { loadingMessage.textContent = "No bus lines have been added yet."; return; }
  loadingMessage.textContent = ""; 
  busLines.forEach(line => busLinesContainer.append(createLineElement(line)));
}

async function getBusLines() {
  loadingMessage.textContent = "Loading bus lines…";
  try { 
    const response = await fetch(apiUrl, requestOptions("GET")); 
    if (!response.ok) throw new Error(); 
    displayBusLines(await response.json()); 
  }
  catch { loadingMessage.textContent = "Could not load bus lines. Please try again."; }
}

function startEdit(busLine) {
  editingId = busLine.id; 
  form.driver.value = busLine.driver; 
  form.plate.value = busLine.plate; 
  form.hours.value = busLine.hours;
  form.routeStatus.value = busLine.routeStatus || "Scheduled";
  form.originCity.value = busLine.originCity;
  form.destinationCity.value = busLine.destinationCity;
  submitButton.textContent = "Save changes"; 
  cancelEditButton.hidden = false; 
  setMessage(`Editing ${busLine.plate}.`); 
  form.driver.focus();
}

async function deleteBusLine(id) {
  if (!confirm("Delete this bus line?")) return;
  try { 
    const response = await fetch(`${apiUrl}/${id}`, requestOptions("DELETE")); 
    if (!response.ok) throw new Error(); 
    setMessage("Bus line deleted."); 
    await getBusLines(); 
  }
  catch { setMessage("Could not delete the bus line. Please log in again and retry.", true); }
}

form.addEventListener("submit", async event => {
  event.preventDefault(); 
  const data = Object.fromEntries(new FormData(form)); 

  if (!plateRegex.test(data.plate)) {
    setMessage("Error: Invalid plate format (e.g., 34 AB 1234 or 34 ABC 123).", true);
    console.log("Invalid plate format: ", data.plate);
    return;
  }

  if (!driverRegex.test(data.driver)) {
    setMessage("Error: Invalid driver name format.", true);
    console.log("Invalid driver name format: ", data.driver);
    return; 
  }

  data.hours = formatTimeRange(data.hours) || data.hours;

  const routeStatuses = ["Scheduled", "En Route", "Delayed", "Cancelled"];
  if (!routeStatuses.includes(data.routeStatus)) {
    setMessage("Error: Invalid route status.", true);
    return;
  }

  data.originCity = data.originCity.trim();
  data.destinationCity = data.destinationCity.trim();
  if (!data.originCity || !data.destinationCity) {
    setMessage("Error: Origin and destination cities are required.", true);
    return;
  }
  
  if (!hoursRegex.test(data.hours)) {
    setMessage("Error: Invalid hours format (e.g., 09:00 - 17:00).", true);
    console.log("Invalid hours format: ", data.hours);
    return; 
  }

  const url = editingId ? `${apiUrl}/${editingId}` : apiUrl; 
  const method = editingId ? "PUT" : "POST";
  
  try { 
    const response = await fetch(url, requestOptions(method, data)); 
    if (!response.ok) throw new Error(); 
    setMessage(editingId ? "Bus line updated." : "Bus line added."); 
    resetForm(); 
    await getBusLines();
  }
  catch { setMessage("Could not save the bus line. Please log in again and retry.", true); }
});

cancelEditButton.addEventListener("click", () => { resetForm(); setMessage(""); });
document.getElementById("refresh-button").addEventListener("click", getBusLines);

logoutButton.addEventListener("click", () => { 
  localStorage.removeItem("busLinesToken"); 
  resetForm(); 
  updateAuthUi(); 
  getBusLines(); 
});

updateAuthUi();
if (getRoleFromToken(getToken() || "") === "Admin") getBusLines();