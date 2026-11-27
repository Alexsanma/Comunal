# Comunal – Microservicios

Plataforma para que una comunidad comparta objetos mediante préstamos. La solución está formada
por **tres microservicios** en **.NET 8**, cada uno con su propia base de datos (SQL Server + EF Core),
y una **interfaz web** que los consume.

| Proyecto | Responsabilidad | Puerto (http) | Base de datos |
|---|---|---|---|
| **Comunal.Web** | Interfaz de usuario (HTML + CSS + JavaScript) | 5080 | — |
| **Identity** | Autenticación con ASP.NET Core Identity y emisión de **JWT** | 5100 | ComunalIdentity |
| **Catálogo** | Objetos de la comunidad (Clean Architecture + DDD + CQRS) | 5090 | ComunalCatalogo |
| **Préstamos** | Solicitar, aprobar y devolver préstamos | 5110 | ComunalPrestamos |

```
                 ┌──────────────────────┐
                 │  Comunal.Web (5080)  │   navegador
                 └──────────┬───────────┘
                HTTP + JWT  │
      ┌─────────────────────┼──────────────────────┐
      ▼                     ▼                      ▼
┌───────────┐        ┌────────────┐  HTTP+JWT ┌────────────┐
│ Identity  │        │  Catálogo  │◄──────────│ Préstamos  │
│  (5100)   │        │   (5090)   │ síncrona  │   (5110)   │
└───────────┘        └────────────┘           └────────────┘
```

## Comunicación síncrona entre microservicios

**Préstamos** llama a **Catálogo** por HTTP (request/response) con un `HttpClient` tipado
(`src/Comunal.Prestamos/Clients/CatalogoClient.cs`) y reenvía el JWT del usuario, así que el
Catálogo vuelve a validar el token y el rol:

| Acción en Préstamos | Llamada a Catálogo | Para qué |
|---|---|---|
| `POST /api/prestamos` (solicitar) | `GET /api/objetos/{id}` | Verifica que el objeto exista (404) y esté **Disponible** (400) |
| `PUT /api/prestamos/{id}/aprobar` | `PUT /api/objetos/{id}/prestar` | Marca el objeto como **Prestado** |
| `PUT /api/prestamos/{id}/devolver` | `PUT /api/objetos/{id}/devolver` | Deja el objeto **Disponible** de nuevo |
| `GET /api/prestamos`, `/mios` | `GET /api/objetos` | Muestra el nombre del objeto en cada préstamo |

Si el Catálogo no responde, Préstamos devuelve **503** con un mensaje claro. Cada llamada queda
registrada en la consola de Préstamos (`Préstamos -> Catálogo: ...`).

## Seguridad

- **Autenticación:** `POST /api/auth/login` en Identity devuelve el JWT.
- **Autorización por roles:** `[Authorize]` y `[Authorize(Roles = "Administrador")]`.
- **401 vs 403:** sin token o token inválido/expirado → **401**; sin el rol requerido → **403**.
- **Contraseñas:** se guardan con el hash seguro de ASP.NET Core Identity.
- **CORS:** los tres microservicios aceptan peticiones del origen de la interfaz (`http://localhost:5080`).

### Usuarios sembrados

| Correo | Contraseña | Rol |
|---|---|---|
| admin@comunal.com | `Admin123$` | Administrador |
| miembro@comunal.com | `Miembro123$` | Miembro |

## Cómo ejecutar

Requisitos: **.NET SDK 8.0** y **SQL Server LocalDB**. Cada microservicio crea y siembra su base
de datos al iniciar. Abre **cuatro** terminales en la raíz del repositorio:

```bash
dotnet run --project src/Comunal.Identity --launch-profile http
dotnet run --project src/Comunal.Catalogo.Api --launch-profile http
dotnet run --project src/Comunal.Prestamos --launch-profile http
dotnet run --project src/Comunal.Web --launch-profile http
```

Luego abre **http://localhost:5080**. Swagger de cada servicio: `http://localhost:{puerto}/swagger`.

## Interfaz web

- **Iniciar sesión** (Identity) con cualquiera de los usuarios sembrados.
- **Catálogo:** lista los objetos y su estado; botón *Solicitar préstamo* en los disponibles.
- **Mis préstamos:** préstamos del usuario; *Devolver* en los aprobados.
- **Administrar préstamos** (solo Administrador): *Aprobar* y *Registrar devolución*.
- **Actividad entre servicios:** panel con cada llamada HTTP que hace la interfaz y su código de respuesta.

## Escenarios del video (entrega final)

1. Arquitectura y ejecución de los cuatro proyectos.
2. Login con credenciales inválidas → 401; login correcto como **Miembro**.
3. Catálogo cargado desde el microservicio Catálogo.
4. Solicitar un préstamo → Préstamos consulta al Catálogo (comunicación síncrona) → 201.
5. Regla de negocio: objeto con solicitud pendiente → 400.
6. Login como **Administrador** → aprobar → el objeto pasa a **Prestado** en el Catálogo.
7. Consola de Préstamos con las llamadas `Préstamos -> Catálogo`.
8. Registrar devolución → el objeto vuelve a **Disponible**.

## Integrantes

- Sara Arciniegas Villa
- Juan Pablo Robledo Urrego
- Juan Pablo Vásquez Tobón
- Alexander Sanmartín Arredondo
