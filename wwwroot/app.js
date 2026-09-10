const apiUrl = "/api/bus-lines";
const busLinesContainer = document.getElementById("bus-lines");
const loadingMessage = document.getElementById("loading-message");
const loginLink = document.getElementById("login-link");
const logoutButton = document.getElementById("logout-button");
const adminBusLinesLink = document.getElementById("admin-bus-lines-link");
const fleetCount = document.getElementById("fleet-count");
const availableCount = document.getElementById("available-count");

const getToken = () => localStorage.getItem("busLinesToken");

function getRoleFromToken(jwt) {
  try {
    const payload = JSON.parse(atob(jwt.split(".")[1].replace(/-/g, "+").replace(/_/g, "/")));
    return payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || payload.role || null;
  } catch { return null; }
}

function updateAuthUi() {
  const authenticated = Boolean(getToken());
  const admin = getRoleFromToken(getToken() || "") === "Admin";
  loginLink.hidden = authenticated;
  logoutButton.hidden = !authenticated;
  adminBusLinesLink.hidden = !admin;
}

function requestOptions() {
  return getToken() ? { headers: { Authorization: `Bearer ${getToken()}` } } : {};
}

function createLineElement(busLine) {
  const item = document.createElement("article");
  item.className = "bus-line";
  const details = document.createElement("div");

  const plate = document.createElement("h3");
  const plateValue = busLine.plate ?? busLine.Plate ?? "";
  const driverValue = busLine.driver ?? busLine.Driver ?? "";
  const hoursValue = busLine.hours ?? busLine.Hours ?? "";
  const originValue = busLine.originCity ?? busLine.OriginCity ?? "";
  const destinationValue = busLine.destinationCity ?? busLine.DestinationCity ?? "";
  const routeStatusValue = busLine.routeStatus ?? busLine.RouteStatus ?? "Scheduled";

  plate.textContent = plateValue;
  const driver = document.createElement("p");
  driver.textContent = `Driver: ${driverValue}`;
  const hours = document.createElement("p");
  hours.textContent = `Hours: ${hoursValue}`;
  const route = document.createElement("p");
  route.textContent = `Route: ${originValue} → ${destinationValue}`;
  const status = document.createElement("span");
  const normalizedStatus = String(routeStatusValue).toLowerCase().replace(/\s+/g, "-");
  status.className = `status-badge status-${normalizedStatus}`;
  status.textContent = routeStatusValue;

  route.className = "line-route";
  const meta = document.createElement("div");
  meta.className = "line-meta";
  meta.append(driver, hours);
  details.append(status, plate, route, meta);
  item.append(details);
  return item;
}

function displayBusLines(busLines) {
  busLinesContainer.replaceChildren();
  fleetCount.textContent = busLines.length;
  availableCount.textContent = busLines.filter(line => String(line.routeStatus ?? line.RouteStatus ?? "Scheduled").toLowerCase() === "scheduled").length;
  if (!busLines.length) {
    loadingMessage.textContent = "No bus lines have been added yet.";
    return;
  }
  loadingMessage.textContent = "";
  busLines.forEach(line => busLinesContainer.append(createLineElement(line)));
}

async function getBusLines() {
  loadingMessage.textContent = "Loading bus lines…";
  try {
    const response = await fetch(apiUrl, requestOptions());
    if (!response.ok) throw new Error();
    displayBusLines(await response.json());
  } catch {
    loadingMessage.textContent = "Could not load bus lines. Please log in and try again.";
  }
}

logoutButton.addEventListener("click", () => {
  localStorage.removeItem("busLinesToken");
  updateAuthUi();
});
document.getElementById("refresh-button").addEventListener("click", getBusLines);

updateAuthUi();
getBusLines();
