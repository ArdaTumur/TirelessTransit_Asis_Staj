const UsernameRegex = /^[a-zA-Z0-9_]{3,20}$/;
const PasswordRegex = /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*-_=+?*]).{8,}$/;
const form = document.querySelector("form");
const message = document.getElementById("form-message");
form.addEventListener("submit", async event => {
  event.preventDefault(); 
  console.log("This method is working");
  message.textContent = "";
  
  const button = form.querySelector("button"); 
  const data = Object.fromEntries(new FormData(form)); 
  button.disabled = true;

  if (!UsernameRegex.test(data.username)){
    alert("Error: Username must be 3-20 characters long and can only contain letters, numbers, and underscores.");
    console.log("Invalid username format: ", data.username);
    form.username.value = "";
    form.password.value = "";
    return;
  }

  if (!PasswordRegex.test(data.password)){
    alert("Error: Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
    console.log("Invalid password format: ", data.password);
    form.username.value = "";
    form.password.value = "";
    return;
  }

  try {
    const response = await fetch("/api/register", { 
      method: "POST", 
      headers: { 
        "Content-Type": "application/json" 
      }, 
      body: JSON.stringify({
        username: data.username,
        password: data.password,
        role: "User"
      }) 
    });

    if (!response.ok) {
      throw new Error("Could not create the account. The username may already be in use.");
    
    }
    window.location.assign("../login/");
  } 
  catch (error) { 
    message.textContent = error.message; 
  }

  finally { 
    button.disabled = false; 
  }
});
