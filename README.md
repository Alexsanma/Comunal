# Comunal – Microservicios

Plataforma para que una comunidad comparta y recupere objetos. La solución está formada por
**tres microservicios** en **.NET 8**, cada uno con su propia base de datos (SQL Server + EF Core):

| Microservicio | Responsabilidad | Puerto (https / http) | Base de datos |
|---|---|---|---|
| **Identity** | Autenticación con ASP.NET Core Identity y emisión de **JWT** | 7100 / 5100 | ComunalIdentity |
| **Catálogo** | Objetos de la comunidad (Entrega 1), ahora con endpoints protegidos | 7090 / 5090 | ComunalCatalogo |
| **Préstamos** | Préstamos de objetos, con endpoints protegidos | 7110 / 5110 | ComunalPrestamos |

Identity autentica a los usuarios y emite un JWT firmado (HMAC-SHA256) con vigencia y con los
claims de usuario y roles. Catálogo y Préstamos **validan ese JWT** (firma, emisor, audiencia y
vigencia) y autorizan el acceso **por roles**.

## Seguridad

- **Autenticación:** `POST /api/auth/login` en Identity devuelve el JWT.
- **Autorización por roles:** se usan los atributos `[Authorize]` y `[Authorize(Roles = "...")]` de ASP.NET Core.
- **401 vs 403:** sin token (o token inválido/expirado) → **401**; autenticado pero sin el rol requerido → **403**.
- **Contraseñas:** se almacenan con el hash seguro de ASP.NET Core Identity (nunca en texto plano).
- La configuración del JWT (`Jwt` en `appsettings.json`) es la misma en los tres servicios.

### Usuarios sembrados (Seeders)

| Correo | Contraseña | Rol |
|---|---|---|
| admin@comunal.com | `Admin123$` | Administrador |
| miembro@comunal.com | `Miembro123$` | Miembro |

## Cómo ejecutar

Requisitos: **.NET SDK 8.0** y **SQL Server LocalDB**. Cada microservicio crea y siembra su base
de datos al iniciar. Hay que levantar los **tres** (cada uno en su terminal):

```bash
dotnet run --project src/Comunal.Identity   --launch-profile https   # Identity  -> https://localhost:7100/swagger
dotnet run --project src/Comunal.Catalogo.Api --launch-profile https  # Catálogo  -> https://localhost:7090/swagger
dotnet run --project src/Comunal.Prestamos  --launch-profile https    # Préstamos -> https://localhost:7110/swagger
```

En Swagger, usa el botón **Authorize** y pega el token devuelto por el login (sin la palabra
`Bearer`). También puedes abrir la solución completa con `Comunal.slnx`.

## Endpoints principales

**Identity**
- `POST /api/auth/login` — inicia sesión y devuelve el JWT.
- `GET /api/auth/me` — datos del usuario autenticado (requiere token).

**Catálogo** (requiere token; escritura solo rol Administrador)
- `GET /api/objetos`, `GET /api/objetos/{id}`, `GET /api/objetos/categoria/{categoriaId}`
- `POST/PUT/DELETE /api/objetos` → solo Administrador

**Préstamos** (requiere token; aprobar solo rol Administrador)
- `GET /api/prestamos`, `GET /api/prestamos/mios`, `POST /api/prestamos`
- `PUT /api/prestamos/{id}/aprobar` → solo Administrador
- `PUT /api/prestamos/{id}/devolver`

## Pruebas (escenarios del video)

En `docs/` está la colección de Postman (`Comunal.Entrega2.postman_collection.json`). Los escenarios:

1. **Ejecución de los tres microservicios.**
2. **Login** con credenciales válidas → se obtiene el JWT.
3. **Acceso sin token** a un endpoint protegido → **401**.
4. **Acceso con token válido** → **200**.
5. **Autorización por rol:** Administrador en un endpoint de escritura → **201/204**.
6. **Sin el rol requerido:** Miembro en un endpoint de Administrador → **403**.
7. **Credenciales inválidas** en el login → **401**.
8. **Token inválido o expirado** → **401**.

## Integrantes

- Sara Arciniegas Villa
- Juan Pablo Robledo Urrego
- Juan Pablo Vásquez Tobón
- Alexander Sanmartín Arredondo
