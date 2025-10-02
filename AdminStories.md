# 🔧 Admin (Administrador)

## Configuración General

### US-ADM01: Configurar feriados nacionales
**Como** administrador del sistema  
**Quiero** cargar todos los feriados de 2025  
**Para** que no se generen turnos en esos días

**Criterios de aceptación:**
- Cargar feriados manualmente o importar  
- Marcar feriados recurrentes (ej: Navidad)  
- Aplicar a todos los profesionales  
- Regenerar slots si es necesario

---

### US-ADM02: Configurar política de anticipación global
**Como** administrador  
**Quiero** establecer que todos los turnos requieren mínimo 12hs de anticipación  
**Para** evitar turnos de último momento en todo el sistema

**Criterios de aceptación:**
- Configurar horas mínimas de anticipación  
- Aplicar a todos los profesionales  
- Permitir excepciones por profesional  
- Validar en reservas

---

### US-ADM03: Configurar horarios de atención del centro
**Como** administrador  
**Quiero** establecer que el centro atiende de 8 a 20hs  
**Para** que ningún profesional genere turnos fuera de ese rango

**Criterios de aceptación:**
- Definir horario general del centro  
- Aplicar restricción a todos los profesionales  
- Validar en configuración de horarios

---

## Gestión de Usuarios

### US-ADM04: Dar de alta profesional nuevo
**Como** administrador  
**Quiero** crear el perfil del Dr. Martínez con su especialidad y matrícula  
**Para** que pueda empezar a atender pacientes

**Criterios de aceptación:**
- Formulario completo de datos profesionales  
- Asignar especialidad(es)  
- Validar matrícula profesional  
- Generar credenciales de acceso

---

### US-ADM05: Dar de alta secretaria
**Como** administrador  
**Quiero** crear usuario para la nueva secretaria Laura  
**Para** que pueda gestionar turnos de todos los profesionales

**Criterios de aceptación:**
- Crear usuario con rol Schedule Manager  
- Asignar permisos de gestión de turnos  
- Configurar acceso a qué profesionales puede gestionar

---

### US-ADM06: Desactivar profesional
**Como** administrador  
**Quiero** desactivar al Dr. Gómez que ya no trabaja aquí  
**Para** que no aparezca en búsquedas pero mantener su historial

**Criterios de aceptación:**
- Marcar como inactivo (no eliminar)  
- No mostrar en búsquedas de pacientes  
- Mantener historial de consultas  
- Cancelar turnos futuros pendientes

---

### US-ADM07: Resetear contraseña de usuario
**Como** administrador  
**Quiero** resetear la contraseña del paciente Rodríguez  
**Porque** olvidó su clave de acceso

**Criterios de aceptación:**
- Generar contraseña temporal  
- Enviar por email al usuario  
- Forzar cambio en próximo login  
- Registrar acción en log

---

## Monitoreo y Reportes

### US-ADM08: Ver dashboard general
**Como** administrador  
**Quiero** ver un dashboard con turnos del día, ocupación y ausentismo  
**Para** monitorear el funcionamiento del sistema

**Criterios de aceptación:**
- KPIs principales en dashboard  
- Gráficos de tendencias  
- Alertas de problemas  
- Actualización en tiempo real

---

### US-ADM09: Generar reporte de facturación
**Como** administrador  
**Quiero** exportar todas las consultas del mes por obra social  
**Para** facturar a las prepagas

**Criterios de aceptación:**
- Filtrar por obra social y período  
- Incluir datos de profesional y paciente  
- Exportar a Excel/CSV  
- Formato compatible con facturación

---

### US-ADM10: Ver log de cambios en turnos
**Como** administrador  
**Quiero** ver quién modificó/canceló turnos en las últimas 24hs  
**Para** auditar operaciones sospechosas

**Criterios de aceptación:**
- Log completo de operaciones  
- Filtrar por usuario, acción, fecha  
- Ver detalles de cada cambio  
- Exportar log

---

### US-ADM11: Configurar slots masivamente
**Como** administrador  
**Quiero** generar turnos para los próximos 3 meses de todos los profesionales  
**Después de** cambiar la configuración global

**Criterios de aceptación:**
- Seleccionar todos o grupo de profesionales  
- Definir período de generación  
- Preview antes de confirmar  
- Proceso en background con notificación

---

## Obras Sociales y Especialidades

### US-ADM12: Gestionar obras sociales
**Como** administrador  
**Quiero** agregar/editar obras sociales que acepta el centro  
**Para** que los pacientes puedan registrar su cobertura

**Criterios de aceptación:**
- CRUD completo de obras sociales  
- Nombre, tipo, descripción  
- Asignar a profesionales que la aceptan

---

### US-ADM13: Gestionar especialidades
**Como** administrador  
**Quiero** agregar nuevas especialidades médicas  
**Para** clasificar correctamente a los profesionales

**Criterios de aceptación:**
- CRUD de especialidades  
- Nombre, descripción, imagen  
- Asociar a profesionales

---

### US-ADM14: Configurar motivos de consulta por especialidad
**Como** administrador  
**Quiero** definir que Cardiología tiene motivos "Control", "Urgencia", "Post-operatorio"  
**Para** categorizar mejor las consultas

**Criterios de aceptación:**
- Asignar motivos a especialidades  
- Paciente ve solo motivos relevantes  
- Estadísticas por motivo
