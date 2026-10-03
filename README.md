Subastas Ya - Plataforma transaccional en Tiempo Real

Plataforma de subastas online desarrollada con arquitectura ACID, gestión de billetera virtual y sincronización en tiempo real. El sistema garantiza la integridad financiera mediante concurrencia optimista y un Transaction Ledger inmutable.

Tecnologías Utilizadas

**Backend:**
* C# / ASP.NET Core API

* Entity Framework Core (v9.0.0): Code first.

* Pomelo.EntityFrameworkCore.MySql (v9.0.0)

* Microsoft.AspNetCore.SignalR (WebSockets): Implementación de WebSockets para comunicación bidireccional en tiempo real

* Autenticación JWT (JSON Web Tokens)

* BCrypt.Net-Next (Hashing de contrasenas)

**Frontend:**

* HTML5, CSS3 (Arquitectura modular)

* Vanilla JavaScript (ES6): Lógica de cliente, Fetch API y modularización sin frameworks pesados.

* Flatpickr: Librería ligera para selección interactiva de fechas y horas.

* SignalR Client: Consumo de WebSockets directamente desde el navegador

**Base de Datos:**

* MySQL: Motor relacional con control de transacciones y normativas ACID


**Configuración y Ejecución del Backend**
1. Configurar la Base de Datos:

   Abrir el archivo appsettings.json ubicado en la raíz del proyecto backend (SubastaYa.Api) y configurar cadena de conexión a MySQL. 
   Reemplazar los valores de Server, User, Password y Database con tus credenciales locales:


{
"ConnectionStrings": {
"DefaultConnection": "Server=localhost;Port=3306;Database=SubastasYaDB;User=root;Password=tu_contrasena;"
},
"Jwt": {
"Key": "ClaveSecretaSeguraDe32Caracteres",
"Issuer": "SubastasYa"
}
}


2. Ejecutar Migraciones (Entity Framework Core)

   Para crear la base de datos y sus tablas a partir de los modelos de C#, abrir la terminal en la carpeta del proyecto backend (SubastaYa.Api) y ejecutar los siguientes comandos usando la CLI de .NET:

- dotnet tool install --global dotnet-ef (Verificar que las herramientas de EF estén instaladas)


- dotnet ef migrations add InitialCreate (Generar la migración inicial (si aún no existe))


- dotnet ef database update (Aplicar las migraciones a la base de datos MySQL)

(Alternativa en Visual Studio: Abrir la Consola del Administrador de Paquetes y ejecutar Add-Migration InitialCreate seguido de Update-Database).

3. Levantar la API

   Desde la terminal, ejecutar el proyecto:

- dotnet run

La API quedará escuchando en https://localhost:7281 (o el puerto configurado en Properties/launchSettings.json).

**Pruebas de Estrés y Concurrencia (Sniping)**

El proyecto incluye un mecanismo de Concurrencia Optimista a nivel de base de datos para prevenir race conditions cuando múltiples usuarios pujan simultáneamente por la misma subasta en el último segundo.

Para probar esta arquitectura, el proyecto incluye un script de validación en Bash.

Ejecución de la prueba:
Inicia sesión con dos usuarios diferentes y extrae sus tokens JWT desde el Local Storage del navegador (tecla F12 -> Application -> Local Storage).

Edita el archivo stress_test_sniping.sh y pega ambos tokens en el arreglo correspondiente.

Desde una terminal Git Bash, ejecutar el script:

bash stress_test_sniping.sh
Resultado esperado: El sistema procesará exitosamente una sola petición (200 OK) y rechazará inmediatamente las demás (409 Conflict), revirtiendo cualquier cobro duplicado en la billetera mediante la clase TransactionScope.
