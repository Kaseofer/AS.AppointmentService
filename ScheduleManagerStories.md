# 📋 Schedule Manager (Secretaria)

## Gestión de Turnos

### US-SM01: Agendar turno manualmente
**Como** secretaria María  
**Quiero** agendar un turno para Juan Pérez con el Dr. García el jueves a las 15hs  
**Porque** el paciente llamó por teléfono y no usa la app

**Criterios de aceptación:**
- Buscar paciente existente o crear nuevo  
- Ver disponibilidad del profesional  
- Seleccionar slot disponible  
- Registrar motivo de consulta  
- Confirmar obra social

---

### US-SM02: Reagendar turno
**Como** secretaria María  
**Quiero** cambiar el turno del paciente López del lunes 10hs al martes 14hs  
**Porque** el paciente solicitó el cambio por teléfono

**Criterios de aceptación:**
- Buscar turno existente  
- Ver nuevos horarios disponibles  
- Mover turno sin perder información  
- Notificar al paciente del cambio

---

### US-SM03: Ver agenda de todos los profesionales
**Como** secretaria María  
**Quiero** ver la agenda del día de todos los médicos en una sola pantalla  
**Para** coordinar las consultas y la sala de espera

**Criterios de aceptación:**
- Vista consolidada de todos los profesionales  
- Filtrar por especialidad  
- Ver estado de cada turno (confirmado, en espera, atendido)  
- Actualización en tiempo real

---

### US-SM04: Confirmar asistencia de pacientes
**Como** secretaria María  
**Quiero** marcar qué pacientes confirmaron asistencia por teléfono  
**Para** saber quiénes probablemente vendrán

**Criterios de aceptación:**
- Marcar turno como "confirmado"  
- Registrar medio de confirmación (teléfono, email, app)  
- Ver lista de turnos sin confirmar del día  
- Enviar recordatorios automáticos

---

### US-SM05: Lista de espera
**Como** secretaria María  
**Quiero** agregar a un paciente a lista de espera para el Dr. Pérez  
**Para** ofrecerle el turno si hay cancelación

**Criterios de aceptación:**
- Agregar paciente a lista de espera  
- Indicar rango de fechas/horarios preferidos  
- Notificar automáticamente si se libera turno  
- Orden de prioridad configurable

---

## Gestión de Profesionales

### US-SM06: Configurar horarios de profesional nuevo
**Como** secretaria María  
**Quiero** configurar los horarios de la nueva Dra. Fernández (Lunes y Miércoles 14-19hs)  
**Para** que se generen sus turnos automáticamente

**Criterios de aceptación:**
- Seleccionar días de la semana  
- Definir horario de inicio y fin  
- Configurar duración de turnos  
- Generar slots automáticamente

---

### US-SM07: Generar slots para período específico
**Como** secretaria María  
**Quiero** generar manualmente los turnos de marzo para el Dr. García  
**Porque** cambió su disponibilidad y necesito regenerar

**Criterios de aceptación:**
- Seleccionar profesional y rango de fechas  
- Visualizar slots antes de confirmar  
- Opción de borrar y regenerar  
- No afectar turnos ya reservados

---

### US-SM08: Bloquear agenda por emergencia
**Como** secretaria María  
**Quiero** cancelar todos los turnos del Dr. Pérez de esta tarde  
**Porque** tuvo una emergencia familiar

**Criterios de aceptación:**
- Seleccionar profesional y horario  
- Ver lista de turnos afectados  
- Notificar a todos los pacientes  
- Ofrecer reagendar automáticamente

---

### US-SM09: Ver reportes de ocupación
**Como** secretaria María  
**Quiero** ver qué profesionales tienen más/menos turnos ocupados  
**Para** balancear la carga entre médicos

**Criterios de aceptación:**
- Reporte de ocupación por profesional  
- Filtrar por período (día, semana, mes)  
- Gráficos de ocupación  
- Exportar a Excel
