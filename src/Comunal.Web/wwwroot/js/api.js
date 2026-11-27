// Cliente HTTP de la interfaz: agrega el JWT, registra cada llamada en el panel
// "Actividad entre servicios" y traduce los errores (401, 403, 400...) a mensajes.
const Api = (() => {
    const cfg = window.COMUNAL_CONFIG;
    const SERVICIOS = {
        identity: { nombre: "Identity", url: cfg.identityUrl },
        catalogo: { nombre: "Catálogo", url: cfg.catalogoUrl },
        prestamos: { nombre: "Préstamos", url: cfg.prestamosUrl }
    };

    let sesion = JSON.parse(sessionStorage.getItem("comunal.sesion") || "null");

    function guardarSesion(nueva) {
        sesion = nueva;
        if (nueva) sessionStorage.setItem("comunal.sesion", JSON.stringify(nueva));
        else sessionStorage.removeItem("comunal.sesion");
    }

    function registrar(servicio, metodo, ruta, status) {
        const lista = document.getElementById("actividad");
        if (!lista) return;
        const item = document.createElement("li");
        const ok = status >= 200 && status < 300;
        item.innerHTML =
            `<span class="hora">${new Date().toLocaleTimeString()}</span>` +
            `<span class="servicio">UI → ${servicio}</span>` +
            `<code>${metodo} ${ruta}</code>` +
            `<span class="status ${ok ? "ok" : "error"}">${status || "sin respuesta"}</span>`;
        lista.prepend(item);
    }

    async function llamar(clave, metodo, ruta, cuerpo) {
        const servicio = SERVICIOS[clave];
        const headers = { "Content-Type": "application/json" };
        if (sesion?.token) headers.Authorization = `Bearer ${sesion.token}`;

        let respuesta;
        try {
            respuesta = await fetch(servicio.url + ruta, {
                method: metodo,
                headers,
                body: cuerpo ? JSON.stringify(cuerpo) : undefined
            });
        } catch {
            registrar(servicio.nombre, metodo, ruta, 0);
            throw new Error(`No se pudo contactar el microservicio ${servicio.nombre}. ¿Está en ejecución?`);
        }

        registrar(servicio.nombre, metodo, ruta, respuesta.status);
        const texto = await respuesta.text();
        const datos = texto ? JSON.parse(texto) : null;

        if (!respuesta.ok) {
            let mensaje = datos?.mensaje || datos?.message || datos?.title;
            if (respuesta.status === 401) mensaje = mensaje || "No autenticado (401): inicia sesión de nuevo.";
            if (respuesta.status === 403) mensaje = "Acceso denegado (403): tu rol no tiene permiso para esta acción.";
            const error = new Error(mensaje || `Error ${respuesta.status}`);
            error.status = respuesta.status;
            throw error;
        }
        return datos;
    }

    return {
        get sesion() { return sesion; },
        esAdmin: () => !!sesion?.roles?.includes("Administrador"),

        async login(email, password) {
            const datos = await llamar("identity", "POST", "/api/auth/login", { email, password });
            guardarSesion(datos);
            return datos;
        },
        logout: () => guardarSesion(null),

        objetos: () => llamar("catalogo", "GET", "/api/objetos"),
        prestamos: () => llamar("prestamos", "GET", "/api/prestamos"),
        misPrestamos: () => llamar("prestamos", "GET", "/api/prestamos/mios"),
        solicitar: (objetoId) => llamar("prestamos", "POST", "/api/prestamos", { objetoId }),
        aprobar: (id) => llamar("prestamos", "PUT", `/api/prestamos/${id}/aprobar`),
        devolver: (id) => llamar("prestamos", "PUT", `/api/prestamos/${id}/devolver`)
    };
})();