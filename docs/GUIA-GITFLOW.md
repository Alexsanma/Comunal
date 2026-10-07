# Guía de Git Flow – Entrega 1 (microservicio Catálogo)

Esta guía explica cómo subir el proyecto al repositorio demostrando **Git Flow** y **Pull Requests
entre integrantes** (lo que pide el entregable 1a: un grafo de red con la colaboración del equipo).

El código completo ya está hecho y repartido en la carpeta `_reparto/` (que **no** se sube al repo).
Cada integrante toma **solo su carpeta** y la agrega en su propia rama.

## Modelo de ramas

```
main        → versión estable (lo que se entrega)
 └── develop → rama de integración
      ├── feature/estructura-base      (Alexander)
      ├── feature/aplicacion-casos-uso (Sara)
      ├── feature/infraestructura      (Robledo)
      └── feature/api                  (Tobón)
```

Cada `feature/*` se abre desde `develop`, se sube, y se integra con un **Pull Request** que
**revisa y aprueba otro compañero**. Al final, un PR de `develop` → `main`.

## Reparto por integrante

| Integrante | Rama | Carpeta en `_reparto/` | Qué entrega |
|---|---|---|---|
| **Alexander** | `feature/estructura-base` | `00-alexander-base` | Solución, los 4 proyectos, capa de Dominio y contratos (interfaces, DTOs) |
| **Sara** | `feature/aplicacion-casos-uso` | `01-sara-aplicacion` | Commands y Queries (CQRS) con sus handlers |
| **Robledo** | `feature/infraestructura` | `02-robledo-infraestructura` | EF Core, repositorios, Unit of Work y migración |
| **Tobón** | `feature/api` | `03-tobon-api` | Controller, Swagger, middleware de errores y `Program.cs` |

**Orden:** primero la base. Luego Sara y Robledo pueden ir en paralelo. Tobón integra de último
(su capa usa las de Sara y Robledo).

---

## Paso 0 — Alexander: crear el repo y subir la base

1. Crear un repositorio **vacío** en GitHub llamado `Comunal` (público).
2. Desde la carpeta `_reparto/00-alexander-base/`:

```bash
git init
git add .
git commit -m "chore: estructura base, dominio y contratos del microservicio Catálogo"
git branch -M main
git remote add origin https://github.com/Alexsanma/Comunal.git
git push -u origin main
git checkout -b develop
git push -u origin develop
```

A partir de aquí, `develop` es la rama de trabajo.

---

## Paso 1 — Sara: capa de Aplicación

```bash
git clone https://github.com/Alexsanma/Comunal.git
cd Comunal
git checkout develop
git checkout -b feature/aplicacion-casos-uso
```
Copiar el contenido de `01-sara-aplicacion/` dentro del repo (respeta las rutas `src/...`) y luego:
```bash
git add .
git commit -m "feat(application): casos de uso CQRS (commands y queries) de objetos"
git push -u origin feature/aplicacion-casos-uso
```
Abrir el PR en GitHub: **base = develop**, **compare = feature/aplicacion-casos-uso**.
Lo revisa y aprueba **Robledo**.

---

## Paso 2 — Robledo: capa de Infraestructura

```bash
git clone https://github.com/Alexsanma/Comunal.git   # o git pull si ya lo tienes
cd Comunal
git checkout develop
git checkout -b feature/infraestructura
```
Copiar el contenido de `02-robledo-infraestructura/` y luego:
```bash
git add .
git commit -m "feat(infrastructure): EF Core, repositorios, Unit of Work y migración inicial"
git push -u origin feature/infraestructura
```
PR: **base = develop**, **compare = feature/infraestructura**. Lo revisa y aprueba **Sara**.

> La migración ya viene incluida. Si prefieres regenerarla:
> `dotnet ef migrations add InitialCreate -p src/Comunal.Catalogo.Infrastructure -s src/Comunal.Catalogo.Api`

---

## Paso 3 — Tobón: capa de API

Hazlo cuando las ramas de Sara y Robledo ya estén en `develop`.
```bash
cd Comunal
git checkout develop
git pull
git checkout -b feature/api
```
Copiar el contenido de `03-tobon-api/` (reemplaza el `Program.cs` base por el completo) y luego:
```bash
git add .
git commit -m "feat(api): endpoints REST, Swagger y manejo de errores de dominio"
git push -u origin feature/api
```
PR: **base = develop**, **compare = feature/api**. Lo revisa y aprueba **Alexander**.

---

## Paso 4 — Cierre: develop → main

Cuando las 4 ramas estén integradas en `develop` y el proyecto corra:
```bash
git checkout develop
git pull
```
Abrir un PR final **base = main**, **compare = develop** con el mensaje
`release: Entrega 1 - microservicio Catálogo` y mergearlo.

---

## Verificar el grafo de red (lo que revisa el profe)

En GitHub: pestaña **Insights → Network**. Debe verse `main` y `develop`, las cuatro ramas
`feature/*` saliendo de `develop` y volviendo a ella por los merges de los PRs, cada una con un
autor distinto. En la pestaña **Pull requests → Closed** quedan los PRs con quién los revisó.

## Comprobar que todo corre (tras integrar)

```bash
dotnet run --project src/Comunal.Catalogo.Api --launch-profile https
```
Abrir `https://localhost:7090/swagger`.
