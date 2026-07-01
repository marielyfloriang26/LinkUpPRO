# Guía de Implementación de Seguridad - LinkUp Pro

Esta guía detalla los pasos obligatorios para integrar la seguridad centralizada en los módulos del proyecto. El objetivo es cumplir con los requerimientos técnicos de **LinkUp Pro** manteniendo un código limpio y protegido.


--------- La seguridad ya está configurada a nivel global. Para sus módulos, lo único que necesitan es implementar la interfaz IEntidadPropietaria y validar con _authorizationService antes de cualquier POST. Todo lo demás (cookies, errores, archivos, fuerza bruta) ya está resuelto en el core." ---------


---

## 1. Protección de Recursos (Autoría)
Cualquier entidad que tenga un dueño (Publicaciones, Comentarios, Partidas, etc.) debe estar protegida para evitar la manipulación de IDs.

### Paso A: Implementar la interfaz
En tu entidad, implementa `IEntidadPropietaria`. Esto permite que el sistema de seguridad identifique automáticamente quién es el dueño.

```csharp
// Ejemplo en Publicacion.cs
public class Publicacion : IEntidadPropietaria 
{
    public int Id { get; set; }
    public string Contenido { get; set; }
    public int UsuarioId { get; set; } // Identificador del autor
} 



### Paso B. Validar controladores

Primero, inyecta [IAuthorizationService] en el constructor de tu controlador. Luego, valida la propiedad antes de realizar cualquier edición o borrado.

Ejemplo de implementación:
C#
[HttpPost]
[ValidateAntiForgeryToken] // REQUERIDO para prevenir ataques CSRF
public async Task<IActionResult> Edit(int id, ViewModel model)
{
    var recurso = await _service.GetByIdAsync(id);
    
    // Validamos: ¿El usuario logueado es el dueño del recurso?
    var authResult = await _authorizationService.AuthorizeAsync(User, recurso, "PropietarioPolicy");

    if (!authResult.Succeeded)
    {
        return Forbid(); // Acceso denegado: el usuario no tiene permisos
    }

    // Si llega aquí, es el dueño. Continuar con la lógica...
}






## 2. Gestión de Archivos
No implementes lógica manual para subir archivos. Utiliza el servicio global IFileService.

Validaciones incluidas: Tamaño máximo (5MB), extensiones permitidas (.jpg, .jpeg, .png, .webp) y nombres aleatorios (GUID).

Uso:

C#
// En tu controlador:
try {
    string rutaImagen = await _fileService.UploadFileAsync(model.Imagen, "uploads/publicaciones");
} catch (Exception ex) {
    ModelState.AddModelError("", ex.Message);
    return View(model);
}








## 3. Reglas de Oro para Controladores

Para que el sistema sea seguro, aplica estas reglas en todos tus controladores:

Protección Global: Todos los controladores de funcionalidades internas deben llevar [Authorize(Policy = "CuentaActiva")].

ViewModels: Nunca recibas entidades de base de datos directamente en los métodos POST. Usa ViewModels para evitar la asignación indebida de campos sensibles.

Filtros de Errores: No uses try-catch para mostrar detalles técnicos al usuario. El sistema ya cuenta con un GlobalExceptionFilter que mostrará mensajes amigables automáticamente.

Validaciones GET/POST:

GET: Valida la propiedad antes de mostrar el formulario de edición.

POST: Valida la propiedad antes de aplicar cualquier cambio en la base de datos. 






----- PROMPT PARA LA IA -----

"Actúa como un desarrollador experto en ASP.NET Core. Adjunto este archivo SEGURIDAD.md que contiene los estándares de seguridad de mi proyecto LinkUp Pro. Por favor, revisa mi controlador actual [PEGA AQUÍ SU CÓDIGO] y ajusta mis métodos para que cumplan estrictamente con las reglas de seguridad definidas en el archivo, sin modificar mi lógica de negocio original."