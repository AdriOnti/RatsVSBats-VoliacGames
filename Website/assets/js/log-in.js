// Inicializar variables
let apiPerfil;
let succesful = false;
let usuarioId;
let usuariosLogin = [];

// Definir elementos del DOM
const loginName = document.getElementById("login-email");
const loginPass = document.getElementById("login-password");
const loginBtn = document.getElementById("loginBtn");
const feedback = document.getElementById("feedback");

// Peticion GET a la API de Usuarios, y persistencia en localStorage
function logindata() {
  fetch(apiUsuarios)
    .then((response) => {
      console.log(response);
      if (response.ok) {
        return response.json();
      }
    })
    .then((data) => {
      console.log(data);
      for (const usuario of data) {
        usuariosLogin.push(usuario);
      }

      RecogerIdUsuario(usuariosLogin);

      apiPerfil = apiPerfiles + "/" + usuarioId;

      storageProfile();
      if(!(usuarioId == null)) {console.log("El id de usuario es: " + usuarioId);}
      

      popupBlock();
    })
    .catch((error) => {
      console.log(error);
    });
}

let gmailLogin = false;
if(localStorage.getItem("rg") == "true"){
  gmailLogin = true;
  logindata();
}

// Comprobar usuario existente y correcto
function RecogerIdUsuario(usuariosLogin) {
  for (const usuario of usuariosLogin) {
    if (gmailLogin){
      if (
        usuario.userEmail == localStorage.getItem("emailG")
      ) {
        console.log(usuario);
        console.log("Hola campeon");
        succesful = true;
        usuarioId = usuario.idUsers;
        break;
      }
    }
    else{
      if (
        usuario.userEmail == loginName.value &&
        usuario.userPassword == loginPass.value
      ) {
        console.log(usuario);
        console.log("Hola campeon");
        succesful = true;
        usuarioId = usuario.idUsers;
        break;
      }
    }
  }

  if (!succesful) {
    console.log("Usuario o contraseña incorrectos");
    loginPass.value = "";
  }
  
}

// Guardar perfil en localStorage
function storageProfile() {
  fetch(apiPerfil)
    .then((response) => {
      //handle response
      console.log(response);
      if (response.ok) {
        return response.json();
      }
    })
    .then((data) => {
      localStorage.setItem("idProfile", data.idProfiles);
      localStorage.setItem("nickname", data.nickname);
      localStorage.setItem("location", data.location);
      localStorage.setItem("completedMissions", data.completedMissions);
      localStorage.setItem("completedBranches", data.completedBranches);
      localStorage.setItem("points", data.points);
    });
}

function popupBlock() {
  if (succesful) {
    document.getElementById("popup-block").style.display = "block";
    document.querySelector("main").style.visibility = "hidden";
  }
}