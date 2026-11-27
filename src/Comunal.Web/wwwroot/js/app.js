// Interfaz principal: inicio de sesión, pestañas y catálogo de objetos.
const App = (() => {
  const $ = (id) => document.getElementById(id);

  function aviso(mensaje, tipo = "ok") {
    const el = document.createElement("div");
    el.className = `aviso ${tipo}`;
    el.textContent = mensaje;
    $("avisos").append(el);
    setTimeout(() => el.remove(), 5000);
  }

  function fecha(valor) {
    if (!valor) return "—";
    // Las fechas se guardan en UTC; si llegan sin zona horaria se interpretan como UTC.
    const utc = /Z|[+-]\d\d:\d\d$/.test(valor) ? valor : `${valor}Z`;
    return new Date(utc).toLocaleString();
  }

  // ---------- Sesión ----------
  function mostrarSesion() {
    const sesion = Api.sesion;
    $("vista-login").classList.toggle("oculto", !!sesion);
    $("vista-app").classList.toggle("oculto", !sesion);
    $("usuario").classList.toggle("oculto", !sesion);
    if (!sesion) return;

    $("usuario-email").textContent = sesion.email;
    $("usuario-rol").textContent = sesion.roles.join(", ");
    document.querySelectorAll(".solo-admin").forEach((el) => el.classList.toggle("oculto", !Api.esAdmin()));
    mostrarTab("catalogo");
  }

  $("form-login").addEventListener("submit", async (e) => {
    e.preventDefault();
    $("login-error").textContent = "";
    try {
      await Api.login($("email").value.trim(), $("password").value);
      $("password").value = "";
      mostrarSesion();
      aviso(`Bienvenido, ${Api.sesion.email}`);
    } catch (err) {
      $("login-error").textContent = err.status === 401 ? "Correo o contraseña incorrectos." : err.message;
    }
  });

  $("btn-salir").addEventListener("click", () => {
    Api.logout();
    mostrarSesion();
  });

  // ---------- Pestañas ----------
  function mostrarTab(nombre) {
    document.querySelectorAll(".pestana").forEach((b) => b.classList.toggle("activa", b.dataset.tab === nombre));
    document.querySelectorAll(".panel").forEach((p) => p.classList.toggle("oculto", p.id !== `tab-${nombre}`));

    if (nombre === "catalogo") cargarCatalogo();
    if (nombre === "mios") window.Prestamos?.cargarMios();
    if (nombre === "admin") window.Prestamos?.cargarTodos();
  }

  document.querySelectorAll(".pestana").forEach((b) => b.addEventListener("click", () => mostrarTab(b.dataset.tab)));

  // ---------- Catálogo ----------
  async function cargarCatalogo() {
    const contenedor = $("lista-objetos");
    try {
      const objetos = await Api.objetos();
      contenedor.innerHTML = "";
      for (const o of objetos) {
        const tarjeta = document.createElement("article");
        tarjeta.className = "objeto";
        tarjeta.innerHTML = `
          <h3>${o.nombre}</h3>
          <span class="categoria">${o.categoria}</span>
          <span class="estado ${o.estado}">${o.estado}</span>`;

        const boton = document.createElement("button");
        boton.className = "btn pequeno";
        boton.textContent = "Solicitar préstamo";
        boton.disabled = o.estado !== "Disponible";
        boton.addEventListener("click", () => solicitar(o));
        tarjeta.append(boton);
        contenedor.append(tarjeta);
      }
    } catch (err) {
      contenedor.innerHTML = `<p class="error">${err.message}</p>`;
    }
  }

  async function solicitar(objeto) {
    try {
      // Préstamos valida el objeto llamando de forma síncrona al Catálogo.
      await Api.solicitar(objeto.id);
      aviso(`Solicitud registrada para «${objeto.nombre}». Queda pendiente de aprobación.`);
    } catch (err) {
      aviso(err.message, "error");
    }
  }

  $("btn-recargar").addEventListener("click", cargarCatalogo);

  document.addEventListener("DOMContentLoaded", mostrarSesion);

  return { aviso, fecha, cargarCatalogo };
})();
