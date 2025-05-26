# EVALUACIÓN TÉCNICA NUXIBA

Prueba: **DESARROLLADOR JR**

Deadline: **1 día**

Nombre: Fernando Ventura Aleman

---

## Clona y crea tu repositorio para la evaluación

1. Clona este repositorio en tu máquina local.
2. Crea un repositorio público en tu cuenta personal de GitHub, BitBucket o Gitlab.
3. Cambia el origen remoto para que apunte al repositorio público que acabas de crear en tu cuenta.
4. Coloca tu nombre en este archivo README.md y realiza un push al repositorio remoto.

---

## Instrucciones Generales

1. Cada pregunta tiene un valor asignado. Asegúrate de explicar tus respuestas y mostrar las consultas o procedimientos que utilizaste.
2. Se evaluará la claridad de las explicaciones, el pensamiento crítico, y la eficiencia de las consultas.
3. Utiliza **SQL Server** para realizar todas las pruebas y asegúrate de que las consultas funcionen correctamente antes de entregar.
4. Justifica tu enfoque cuando encuentres una pregunta sin una única respuesta correcta.
5. Configura un Contenedor de **SQL Server con Docker** utilizando los siguientes pasos:

### Pasos para ejecutar el contenedor de SQL Server

Asegúrate de tener Docker instalado y corriendo en tu máquina. Luego, ejecuta el siguiente comando para levantar un contenedor con SQL Server:

```bash
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=YourStrong!Passw0rd'    -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2019-latest
```

6. Conéctate al servidor de SQL con cualquier herramienta como **SQL Server Management Studio** o **Azure Data Studio** utilizando las siguientes credenciales:
   - **Servidor**: localhost, puerto 1433
   - **Usuario**: SA
   - **Contraseña**: YourStrong!Passw0rd

---

# Examen Práctico para Desarrollador Junior en .NET 8 y SQL Server

**Tiempo estimado:** 1 día  
**Total de puntos:** 100

---

## Instrucciones Generales:

El examen está compuesto por tres ejercicios prácticos. Sigue las indicaciones en cada uno y asegúrate de entregar el código limpio y funcional.

Además, se proporciona un archivo **CCenterRIA.xlsx** para que te bases en la estructura de las tablas y datos proporcionados.

[Descargar archivo de ejemplo](CCenterRIA.xlsx)

---

## Ejercicio 1: API RESTful con ASP.NET Core y Entity Framework (40 puntos)

**Instrucciones:**  
Desarrolla una API RESTful con ASP.NET Core y Entity Framework que permita gestionar el acceso de usuarios.

1. **Creación de endpoints**:
   - **GET /logins**: Devuelve todos los registros de logins y logouts de la tabla `ccloglogin`. (5 puntos)
   - **POST /logins**: Permite registrar un nuevo login/logout. (5 puntos)
   - **PUT /logins/{id}**: Permite actualizar un registro de login/logout. (5 puntos)
   - **DELETE /logins/{id}**: Elimina un registro de login/logout. (5 puntos)

2. **Modelo de la entidad**:  
   Crea el modelo `Login` basado en los datos de la tabla `ccloglogin`:
   - `User_id` (int)
   - `Extension` (int)
   - `TipoMov` (int) → 1 es login, 0 es logout
   - `fecha` (datetime)

3. **Base de datos**:  
   Utiliza **Entity Framework Core** para crear la tabla en una base de datos SQL Server basada en este modelo. Aplica migraciones para crear la tabla en la base de datos. (10 puntos)

4. **Validaciones**:  
   Implementa las validaciones necesarias para asegurar que las fechas sean válidas y que el `User_id` esté presente en la tabla `ccUsers`. Además, maneja errores como intentar registrar un login sin un logout anterior. (10 puntos)

5. **Pruebas Unitarias** (Opcional):  
   Se valorará si incluyes pruebas unitarias para los endpoints de tu API utilizando un framework como **xUnit** o **NUnit**. (Puntos extra)

---

## Ejercicio 2: Consultas SQL y Optimización (30 puntos)

**Instrucciones:**

Trabaja en SQL Server y realiza las siguientes consultas basadas en la tabla `ccloglogin`:

1. **Consulta del usuario que más tiempo ha estado logueado** (10 puntos):
   - Escribe una consulta que devuelva el usuario que ha pasado más tiempo logueado. Para calcular el tiempo de logueo, empareja cada "login" (TipoMov = 1) con su correspondiente "logout" (TipoMov = 0) y suma el tiempo total por usuario.

   Ejemplo de respuesta:  
   - `User_id`: 92  
   - Tiempo total: 361 días, 12 horas, 51 minutos, 8 segundos

```sql
--Consulta del usuario que mas tiempo ha estado logueado
WITH TiempoSegundos AS (
    SELECT Top 1
        User_Id,
        ISNULL(SUM(CASE WHEN TipoMov = 1 THEN CAST(DATEDIFF(SECOND, '2000-01-01', fecha) AS BIGINT) END), 0) -
        ISNULL(SUM(CASE WHEN TipoMov = 0 THEN CAST(DATEDIFF(SECOND, '2000-01-01', fecha) AS BIGINT) END), 0) AS TotalSegundos
    FROM ccloglogin
    GROUP BY User_Id
	Order by TotalSegundos DESC
)
SELECT
    User_Id,
    CAST(TotalSegundos / 31536000 AS VARCHAR) + ' años, ' +
    CAST((TotalSegundos % 31536000) / 2592000 AS VARCHAR) + ' meses, ' +
    CAST(((TotalSegundos % 31536000) % 2592000) / 86400 AS VARCHAR) + ' días, ' +
    CAST((((TotalSegundos % 31536000) % 2592000) % 86400) / 3600 AS VARCHAR) + ' horas, ' +
    CAST(((((TotalSegundos % 31536000) % 2592000) % 86400) % 3600) / 60 AS VARCHAR) + ' minutos, ' +
    CAST(((((TotalSegundos % 31536000) % 2592000) % 86400) % 3600) % 60 AS VARCHAR) + ' segundos'
    AS TiempoFormateado
FROM TiempoSegundos;
```

2. **Consulta del usuario que menos tiempo ha estado logueado** (10 puntos):
   - Escribe una consulta similar a la anterior, pero que devuelva el usuario que ha pasado menos tiempo logueado.

   Ejemplo de respuesta:  
   - `User_id`: 90  
   - Tiempo total: 244 días, 43 minutos, 15 segundos

```sql
--Consulta del usuario que menos tiempo ha estado logueado
WITH TiempoSegundos AS (
    SELECT Top 1
        User_Id,
        ISNULL(SUM(CASE WHEN TipoMov = 1 THEN CAST(DATEDIFF(SECOND, '2000-01-01', fecha) AS BIGINT) END), 0) -
        ISNULL(SUM(CASE WHEN TipoMov = 0 THEN CAST(DATEDIFF(SECOND, '2000-01-01', fecha) AS BIGINT) END), 0) AS TotalSegundos
    FROM ccloglogin
    GROUP BY User_Id
	Order by TotalSegundos ASC
)
SELECT
    User_Id,
    CAST(TotalSegundos / 31536000 AS VARCHAR) + ' años, ' +
    CAST((TotalSegundos % 31536000) / 2592000 AS VARCHAR) + ' meses, ' +
    CAST(((TotalSegundos % 31536000) % 2592000) / 86400 AS VARCHAR) + ' días, ' +
    CAST((((TotalSegundos % 31536000) % 2592000) % 86400) / 3600 AS VARCHAR) + ' horas, ' +
    CAST(((((TotalSegundos % 31536000) % 2592000) % 86400) % 3600) / 60 AS VARCHAR) + ' minutos, ' +
    CAST(((((TotalSegundos % 31536000) % 2592000) % 86400) % 3600) % 60 AS VARCHAR) + ' segundos'
    AS TiempoFormateado
FROM TiempoSegundos;
```

3. **Promedio de logueo por mes** (10 puntos):
   - Escribe una consulta que calcule el tiempo promedio de logueo por usuario en cada mes.

   Ejemplo de respuesta:  
   - Usuario 70 en enero 2023: 3 días, 14 horas, 1 minuto, 16 segundos

```sql
--Promedio de logueo por mes
WITH Movimientos AS (
    SELECT 
        User_Id,
        fecha,
        TipoMov,
        FORMAT(fecha, 'yyyy-MM') AS Periodo,
        ROW_NUMBER() OVER (PARTITION BY User_Id, FORMAT(fecha, 'yyyy-MM'), TipoMov ORDER BY fecha) AS rn
    FROM ccloglogin
),
LogueosEmparejados AS (
    SELECT 
        login.User_Id,
        login.Periodo,
        login.fecha AS FechaLogin,
        logout.fecha AS FechaLogout,
        DATEDIFF(SECOND, login.fecha, logout.fecha) AS DuracionSegundos
    FROM Movimientos login
    INNER JOIN Movimientos logout
        ON login.User_Id = logout.User_Id
        AND login.Periodo = logout.Periodo
        AND login.rn = logout.rn
        AND login.TipoMov = 1 AND logout.TipoMov = 0
    WHERE logout.fecha > login.fecha
),
Promedios AS (
    SELECT 
        User_Id,
        Periodo,
        AVG(DuracionSegundos * 1.0) AS PromedioSegundos
    FROM LogueosEmparejados
    GROUP BY User_Id, Periodo
)
SELECT 
    User_Id,
    Periodo,
    FORMAT(PromedioSegundos / 86400.0, 'N2') + ' días, ' +
    FORMAT((PromedioSegundos % 86400) / 3600.0, 'N2') + ' horas, ' +
    FORMAT((PromedioSegundos % 3600) / 60.0, 'N2') + ' minutos, ' +
    FORMAT(PromedioSegundos % 60.0, 'N2') + ' segundos' AS TiempoPromedio
FROM Promedios
ORDER BY User_Id, Periodo;
```
---

## Ejercicio 3: API RESTful para generación de CSV (30 puntos)

**Instrucciones:**

1. **Generación de CSV**:  
   Crea un endpoint adicional en tu API que permita generar un archivo CSV con los siguientes datos:
   - Nombre de usuario (`Login` de la tabla `ccUsers`)
   - Nombre completo (combinación de `Nombres`, `ApellidoPaterno`, y `ApellidoMaterno` de la tabla `ccUsers`)
   - Área (tomado de la tabla `ccRIACat_Areas`)
   - Total de horas trabajadas (basado en los registros de login y logout de la tabla `ccloglogin`)

   El CSV debe calcular el total de horas trabajadas por usuario sumando el tiempo entre logins y logouts.

2. **Formato y Entrega**:
   - El CSV debe ser descargable a través del endpoint de la API.
   - Asegúrate de probar este endpoint utilizando herramientas como **Postman** o **curl** y documenta los pasos en el archivo README.md.

---

## Entrega

1. Sube tu código a un repositorio en GitHub o Bitbucket y proporciona el enlace para revisión.
2. El repositorio debe contener las instrucciones necesarias en el archivo **README.md** para:
   - Levantar el contenedor de SQL Server.
   - Conectar la base de datos.
   - Ejecutar la API y sus endpoints.
   - Descargar el CSV generado.
3. **Opcional**: Si incluiste pruebas unitarias, indica en el README cómo ejecutarlas.

---

Este examen evalúa tu capacidad para desarrollar APIs RESTful, realizar consultas avanzadas en SQL Server y generar reportes en formato CSV. Se valorará la organización del código, las mejores prácticas y cualquier documentación adicional que proporciones.

---
## 📄 Generar Reporte CSV de Horas Trabajadas

Este endpoint genera un archivo CSV que contiene la lista de usuarios junto con el total de horas trabajadas, calculadas a partir de sus registros de login/logout.

### Probar con Postman

#### Método y URL
`GET http://localhost:{puerto}/generarCSV`

> Reemplaza `{puerto}` con el número de puerto que usa tu aplicación (por ejemplo: `5000`, `7090`, etc.).

#### Headers
No se requieren headers especiales. Asegúrate de que el servidor esté en ejecución.

#### Ejemplo de uso

1. Abre Postman.
2. Crea una nueva solicitud (`New > HTTP Request`).
3. Selecciona el método `GET`.
4. Ingresa la URL del endpoint:
`http://localhost:5000/generarCSV`
5. Haz clic en **Send**.
6. La respuesta será un archivo `.csv`. En Postman, selecciona la pestaña **Body** > **Save to file** para descargarlo.

---

### Contenido del CSV

El archivo contiene las siguientes columnas:

```csv
Login,Nombre Completo,Área,Total Horas Trabajadas
cechavezAgent,cechavezAgent cechavezAgent cechavezAgent,Default,7958.19
charlesAgent,charlesAgent charlesAgent charlesAgent,Default,8349.55
```
###Detalles Técnicos
Se suman las horas entre pares consecutivos de `Login (TipoMov = 1)` y `Logout (TipoMov = 0)`.

El nombre completo se genera concatenando `Nombre`, `ApellidoPaterno` y `ApellidoMaterno`.

Si el usuario no tiene área asignada, se muestra `"Sin área"`.

El resultado se devuelve como un archivo `reporte_horas_trabajadas.csv` con tipo MIME `text/csv`.
