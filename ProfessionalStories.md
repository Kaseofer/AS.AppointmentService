# 👨‍⚕️ Professional (Médico)

## Gestión de Agenda Propia

### US-P01: Ver mi agenda semanal
**Como** Dr. Pérez (profesional)  
**Quiero** ver mi agenda de la semana completa con todos los turnos confirmados  
**Para** planificar mi semana y prepararme para las consultas

**Criterios de aceptación:**
- Visualizar agenda en formato calendario semanal  
- Ver nombre del paciente, hora y motivo de consulta  
- Filtrar por estado (confirmado, pendiente, cancelado)  
- Código de colores por tipo de consulta

---

### US-P02: Ver detalle del paciente antes de la consulta
**Como** Dr. Pérez  
**Quiero** ver el historial de consultas previas de un paciente desde mi agenda  
**Para** revisar su caso antes de atenderlo

**Criterios de aceptación:**
- Click en turno muestra ficha del paciente  
- Ver últimas 5 consultas del paciente  
- Ver obra social y datos de contacto  
- Acceso a observaciones previas

---

### US-P03: Bloquear horario específico
**Como** Dr. Pérez  
**Quiero** bloquear el miércoles 20 de 10-12hs porque tengo una cirugía  
**Para** que no se agenden turnos en ese horario

**Criterios de aceptación:**
- Seleccionar fecha y rango horario a bloquear  
- Indicar motivo del bloqueo  
- Confirmar que no hay turnos ya agendados  
- Notificar a secretaria del bloqueo

---

### US-P04: Solicitar vacaciones
**Como** Dr. Pérez  
**Quiero** marcar mis vacaciones del 15 al 31 de enero  
**Para** que la secretaria no genere turnos en esas fechas

**Criterios de aceptación:**
- Seleccionar rango de fechas  
- Indicar motivo (vacaciones, congreso, etc)  
- Requiere aprobación de admin/secretaria  
- No permitir si hay turnos ya agendados

---

### US-P05: Ver estadísticas de mi agenda
**Como** Dr. Pérez  
**Quiero** ver cuántos pacientes atendí este mes y tasa de ausentismo  
**Para** evaluar mi carga de trabajo

**Criterios de aceptación:**
- Total de consultas del mes/semana  
- Tasa de ausentismo  
- Horarios más solicitados  
- Exportar a PDF

---

### US-P06: Modificar duración de turnos
**Como** Dr. Pérez  
**Quiero** cambiar la duración de mis turnos de 30 a 45 minutos  
**Porque** necesito más tiempo por consulta

**Criterios de aceptación:**
- Modificar duración en configuración personal  
- Aplicar cambio a partir de fecha específica  
- Regenerar slots futuros  
- No afectar turnos ya agendados
