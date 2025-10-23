-- ============================================
-- CONSTRAINTS Y CLAVES - MICROSERVICIO APPOINTMENTS
-- ============================================
-- Proyecto: Sistema de Gestión de Turnos Médicos
-- Microservicio: Appointments
-- Descripción: Primary Keys, Unique Constraints y Foreign Keys
-- ============================================

-- ============================================
-- PRIMARY KEYS (ya definidas en CREATE TABLE)
-- ============================================
-- appointment_status.id (SERIAL)
-- appointment_reason.id (SERIAL)
-- appointment.id (UUID)
-- national_holidays.id (SERIAL)
-- professional_non_working_days.id (SERIAL)
-- professional_schedule.id (SERIAL)
-- appointment_slots.id (UUID)
-- slot_generation_config.id (SERIAL)

-- ============================================
-- UNIQUE CONSTRAINTS
-- ============================================

-- appointment_status
-- Ya tiene: name (UNIQUE en CREATE TABLE)

-- appointment_reason
-- Ya tiene: name (UNIQUE en CREATE TABLE)

-- national_holidays
-- Ya tiene: holiday_date (UNIQUE en CREATE TABLE)

-- slot_generation_config
-- Ya tiene: professional_id (UNIQUE en CREATE TABLE)

-- professional_non_working_days: evitar duplicados
ALTER TABLE professional_non_working_days
    ADD CONSTRAINT unique_professional_non_working_date 
    UNIQUE (professional_id, non_working_date);

-- professional_schedule: evitar duplicados de horarios
ALTER TABLE professional_schedule
    ADD CONSTRAINT unique_professional_schedule 
    UNIQUE (professional_id, day_of_week, start_time, end_time);

-- appointment_slots: evitar slots duplicados
ALTER TABLE appointment_slots
    ADD CONSTRAINT unique_slot_time 
    UNIQUE (professional_id, slot_date, start_time);

-- ============================================
-- FOREIGN KEYS
-- ============================================

-- appointment -> appointment_status
ALTER TABLE appointment
    ADD CONSTRAINT fk_appointment_status 
    FOREIGN KEY (status_id) 
    REFERENCES appointment_status(id) 
    ON DELETE RESTRICT;

-- appointment -> appointment_reason
ALTER TABLE appointment
    ADD CONSTRAINT fk_appointment_reason 
    FOREIGN KEY (reason_id) 
    REFERENCES appointment_reason(id) 
    ON DELETE SET NULL;

-- appointment_slots -> appointment (opcional)
ALTER TABLE appointment_slots
    ADD CONSTRAINT fk_slot_appointment 
    FOREIGN KEY (appointment_id) 
    REFERENCES appointment(id) 
    ON DELETE SET NULL;

-- ============================================
-- CHECK CONSTRAINTS ADICIONALES
-- ============================================

-- appointment: validar que end_time > start_time
ALTER TABLE appointment
    ADD CONSTRAINT check_appointment_time 
    CHECK (end_time > start_time);

-- appointment: validar duración
ALTER TABLE appointment
    ADD CONSTRAINT check_appointment_duration 
    CHECK (duration_minutes IN (10, 15, 20, 30, 45, 60));

-- appointment: validar fecha no sea pasada (al crear)
ALTER TABLE appointment
    ADD CONSTRAINT check_appointment_future_date 
    CHECK (appointment_date >= CURRENT_DATE);

-- appointment_slots: validar que end_time > start_time
ALTER TABLE appointment_slots
    ADD CONSTRAINT check_slot_time 
    CHECK (end_time > start_time);

-- appointment_slots: validar duración
ALTER TABLE appointment_slots
    ADD CONSTRAINT check_slot_duration 
    CHECK (duration_minutes IN (10, 15, 20, 30, 45, 60));

-- professional_non_working_days: no permitir fechas pasadas
ALTER TABLE professional_non_working_days
    ADD CONSTRAINT check_non_working_date 
    CHECK (non_working_date >= CURRENT_DATE);

-- ============================================
-- COMENTARIOS
-- ============================================

COMMENT ON CONSTRAINT fk_appointment_status ON appointment 
    IS 'Relación turno con estado - RESTRICT evita eliminar estados en uso';

COMMENT ON CONSTRAINT fk_appointment_reason ON appointment 
    IS 'Relación turno con motivo - SET NULL mantiene turno aunque se elimine motivo';

COMMENT ON CONSTRAINT fk_slot_appointment ON appointment_slots 
    IS 'Relación slot con turno - SET NULL libera slot al cancelar turno';

COMMENT ON CONSTRAINT unique_professional_non_working_date ON professional_non_working_days 
    IS 'Evita registrar el mismo día no laborable múltiples veces';

COMMENT ON CONSTRAINT unique_professional_schedule ON professional_schedule 
    IS 'Evita horarios duplicados para el mismo profesional y día';

COMMENT ON CONSTRAINT unique_slot_time ON appointment_slots 
    IS 'Evita slots duplicados en el mismo horario';

COMMENT ON CONSTRAINT check_appointment_time ON appointment 
    IS 'Valida que la hora de fin sea posterior a la de inicio';

COMMENT ON CONSTRAINT check_appointment_duration ON appointment 
    IS 'Solo permite duraciones estándar: 10, 15, 20, 30, 45, 60 minutos';

COMMENT ON CONSTRAINT check_appointment_future_date ON appointment 
    IS 'Evita crear turnos en fechas pasadas';
