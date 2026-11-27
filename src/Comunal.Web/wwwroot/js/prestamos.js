// Pestañas de préstamos: "Mis préstamos" (cualquier usuario) y
// "Administrar préstamos" (solo rol Administrador: aprobar).
window.Prestamos = (() => {
  const $ = (id) => document.getElementById(id);

  function fila(celdas) {
    const tr = document.createElement("tr");
    for (const c of celdas) {
      const td = document.createElement("td");
      if (c instanceof Node) td.append(c); else td.innerHTML = c;
      tr.append(td);
    }
    return tr;
  }

  function estado(e) {
    return `<span class="estado ${e}">${e}</span>`;
  }

  function boton(texto, accion, secundario = false) {
    const b = document.createElement("button");
    b.className = `btn pequeno${secundario ? " secundario" : ""}`;
    b.textContent = texto;
    b.addEventListener("click", async () => {
      b.disabled = true;
      try { await accion(); } finally { b.disabled = false; }
    });
    return b;
  }

  function vacio(tbody, columnas, texto) {
    tbody.innerHTML = `<tr><td class="vacio" colspan="${columnas}">${texto}</td></tr>`;
  }

  // ---------- Acciones ----------
  async function aprobar(p) {
    try {
      // Préstamos aprueba y llama de forma síncrona al Catálogo para marcar el objeto como Prestado.
      await Api.aprobar(p.id);
      App.aviso(`Préstamo #${p.id} aprobado. El Catálogo marcó «${p.nombreObjeto}» como Prestado.`);
      await cargarTodos();
    } catch (err) {
      App.aviso(err.message, "error");
    }
  }

  async function devolver(p, recargar) {
    try {
      // Préstamos registra la devolución y el Catálogo vuelve a dejar el objeto Disponible.
      await Api.devolver(p.id);
      App.aviso(`Devolución registrada. «${p.nombreObjeto}» vuelve a estar Disponible en el Catálogo.`);
      await recargar();
    } catch (err) {
      App.aviso(err.message, "error");
    }
  }

  // ---------- Mis préstamos ----------
  async function cargarMios() {
    const tbody = $("tabla-mios");
    try {
      const prestamos = await Api.misPrestamos();
      tbody.innerHTML = "";
      if (!prestamos.length) return vacio(tbody, 5, "Todavía no has solicitado préstamos.");

      for (const p of prestamos) {
        const accion = p.estado === "Aprobado" ? boton("Devolver", () => devolver(p, cargarMios), true) : "";
        tbody.append(fila([p.id, p.nombreObjeto ?? `Objeto ${p.objetoId}`, estado(p.estado), App.fecha(p.fechaSolicitud), accion]));
      }
    } catch (err) {
      vacio(tbody, 5, err.message);
    }
  }

  // ---------- Administrar (solo Administrador) ----------
  async function cargarTodos() {
    const tbody = $("tabla-admin");
    try {
      const prestamos = await Api.prestamos();
      tbody.innerHTML = "";
      if (!prestamos.length) return vacio(tbody, 5, "No hay préstamos registrados.");

      for (const p of prestamos) {
        let accion = "";
        if (p.estado === "Solicitado") accion = boton("Aprobar", () => aprobar(p));
        if (p.estado === "Aprobado") accion = boton("Registrar devolución", () => devolver(p, cargarTodos), true);
        tbody.append(fila([p.id, p.nombreObjeto ?? `Objeto ${p.objetoId}`, p.solicitanteId, estado(p.estado), accion]));
      }
    } catch (err) {
      vacio(tbody, 5, err.message);
    }
  }

  return { cargarMios, cargarTodos };
})();
