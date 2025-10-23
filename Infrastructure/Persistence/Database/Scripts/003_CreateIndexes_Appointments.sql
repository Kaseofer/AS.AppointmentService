-- ============================================
-- ÍNDICES - MICROSERVICIO APPOINTMENTS
-- ============================================
-- Proyecto: Sistema de Gestión de Turnos Médicos
-- Microservicio: Appointments
-- Descripción: Índices para optimizar consultas de turnos
-- ============================================

-- ============================================
-- ÍNDICES - appointment_status
-- ============================================

CREATE INDEX IF NOT EXISTS idx_appointment_status_name 
    ON appointment_status USING btree (name);

CREATE INDEX IF NOT EXISTS idx_appointment_status_active 
    ON appointment_status USING btree (is_active) 
    WHERE (is_active = true);

COMMENT ON INDEX idx_appointment_status_name IS 'Búsqueda de estados por nombre';
COMMENT ON INDEX idx_appointment_status_active IS 'Filtrar estados activos';

-- ============================================
-- ÍNDICES - appointment_reason
-- ============================================

CREATE INDEX IF NOT EXISTS idx_appointment_reason_name 
    ON appointment_reason USING btree (name);

CREATE INDEX IF NOT EXISTS idx_appointment_reason_active 
    ON appointment_reason USING btree (is_active) 
    WHERE (is_active = true);

COMMENT ON INDEX idx_appointment_reason_name IS 'Búsqueda de motivos por nombre';
COMMENT ON INDEX idx_appointment_reason_active IS 'Filtrar motivos activos';

-- ============================================
-- ÍNDICES - appointment
-- ============================================

-- Índice por profesional (consulta más frecuente)
CREATE INDEX IF NOT EXISTS idx_appointment_professional_id 
    ON appointment USING btree (professional_id);

-- Índice por paciente
CREATE INDEX IF NOT EXISTS idx_appointment_patient_id 
    ON appointment USING btree (patient_id);

-- Índice por estado
CREATE INDEX IF NOT EXISTS idx_appointment_status_id 
    ON appointment USING btree (status_id);

-- Índice por fecha (consultas por rango de fechas)
CREATE INDEX IF NOT EXISTS idx_appointment_date 
    ON appointment USING btree (appointment_date);

-- Índice compuesto: profesional + fecha (agenda del día)
CREATE INDEX IF NOT EXISTS idx_appointment_prof_date 
    ON appointment USING btree (professional_id, appointment_date);

-- Índice compuesto: paciente + fecha (historial del paciente)
CREATE INDEX IF NOT EXISTS idx_appointment_patient_date 
    ON appointment USING btree (patient_id, appointment_date DESC);

-- Índice compuesto: profesional + fecha + estado (turnos del día por estado)
CREATE INDEX IF NOT EXISTS idx_appointment_prof_date_status 
    ON appointment USING btree (professional_id, appointment_date, status_id);

-- Índice para turnos futuros
CREATE INDEX IF NOT EXISTS idx_appointment_future 
    ON appointment USING btree (appointment_date, start_time) 
    WHERE (appointment_date >= CURRENT_DATE);

-- Índice para búsqueda por hora de inicio
CREATE INDEX IF NOT EXISTS idx_appointment_start_time 
    ON appointment USING btree (start_time);

-- Índice por fecha de creación (auditoría)
CREATE INDEX IF NOT EXISTS idx_appointment_created_at 
    ON appointment USING btree (created_at DESC);

COMMENT ON INDEX idx_appointment_professional_id IS 'CRÍTICO: Búsqueda de turnos por profesional';
COMMENT ON INDEX idx_appointment_patient_id IS 'Búsqueda de turnos por paciente';
COMMENT ON INDEX idx_appointment_date IS 'Búsqueda por rango de fechas';
COMMENT ON INDEX idx_appointment_prof_date IS 'Agenda diaria del profesional';
COMMENT ON INDEX idx_appointment_future IS 'Optimiza búsqueda de turnos futuros';

-- ============================================
-- ÍNDICES - national_holidays
-- ============================================

CREATE INDEX IF NOT EXISTS idx_national_holidays_date 
    ON national_holidays USING btree (holiday_date);

CREATE INDEX IF NOT EXISTS idx_national_holidays_year 
    ON national_holidays USING btree (EXTRACT(YEAR FROM holiday_date));

COMMENT ON INDEX idx_national_holidays_date IS 'Búsqueda de feriados por fecha';
COMMENT ON INDEX idx_national_holidays_year IS 'Búsqueda de feriados por año';

-- ============================================
-- ÍNDICES - professional_non_working_days
-- ============================================

CREATE INDEX IF NOT EXISTS idx_non_working_professional_id 
    ON professional_non_working_days USING btree (professional_id);

CREATE INDEX IF NOT EXISTS idx_non_working_date 
    ON professional_non_working_days USING btree (non_working_date);

CREATE INDEX IF NOT EXISTS idx_non_working_prof_date 
    ON professional_non_working_days USING btree (professional_id, non_working_date);

CREATE INDEX IF NOT EXISTS idx_non_working_future 
    ON professional_non_working_days USING btree (professional_id, non_working_date) 
    WHERE (non_working_date >= CURRENT_DATE);

COMMENT ON INDEX idx_non_working_professional_id IS 'Días no laborables por profesional';
COMMENT ON INDEX idx_non_working_prof_date IS 'CRÍTICO: Verificar disponibilidad';
COMMENT ON INDEX idx_non_working_future IS 'Solo días no laborables futuros';

-- ============================================
-- ÍNDICES - professional_schedule
-- ============================================

CREATE INDEX IF NOT EXISTS idx_schedule_professional_id 
    ON professional_schedule USING btree (professional_id);

CREATE INDEX IF NOT EXISTS idx_schedule_day_of_week 
    ON professional_schedule USING btree (day_of_week);

CREATE INDEX IF NOT EXISTS idx_schedule_prof_day 
    ON professional_schedule USING btree (professional_id, day_of_week);

CREATE INDEX IF NOT EXISTS idx_schedule_active 
    ON professional_schedule USING btree (professional_id, is_active) 
    WHERE (is_active = true);

COMMENT ON INDEX idx_schedule_professional_id IS 'Horarios por profesional';
COMMENT ON INDEX idx_schedule_prof_day IS 'CRÍTICO: Horario de un día específico';
COMMENT ON INDEX idx_schedule_active IS 'Solo horarios activos';

-- ============================================
-- ÍNDICES - appointment_slots
-- ============================================

-- Índice por profesional
CREATE INDEX IF NOT EXISTS idx_slots_professional_id 
    ON appointment_slots USING btree (professional_id);

-- Índice por fecha
CREATE INDEX IF NOT EXISTS idx_slots_date 
    ON appointment_slots USING btree (slot_date);

-- Índice por disponibilidad
CREATE INDEX IF NOT EXISTS idx_slots_available 
    ON appointment_slots USING btree (is_available) 
    WHERE (is_available = true);

-- Índice compuesto: profesional + fecha (slots del día)
CREATE INDEX IF NOT EXISTS idx_slots_prof_date 
    ON appointment_slots USING btree (professional_id, slot_date);

-- Índice compuesto: profesional + fecha + disponibilidad (slots disponibles del día)
CREATE INDEX IF NOT EXISTS idx_slots_prof_date_available 
    ON appointment_slots USING btree (professional_id, slot_date, is_available) 
    WHERE (is_available = true);

-- Índice compuesto: profesional + fecha + hora (búsqueda exacta)
CREATE INDEX IF NOT EXISTS idx_slots_prof_date_time 
    ON appointment_slots USING btree (professional_id, slot_date, start_time);

-- Índice para slots futuros disponibles
CREATE INDEX IF NOT EXISTS idx_slots_future_available 
    ON appointment_slots USING btree (professional_id, slot_date, start_time) 
    WHERE (is_available = true AND slot_date >= CURRENT_DATE);

-- Índice por appointment_id (slots ocupados)
CREATE INDEX IF NOT EXISTS idx_slots_appointment_id 
    ON appointment_slots USING btree (appointment_id) 
    WHERE (appointment_id IS NOT NULL);

COMMENT ON INDEX idx_slots_professional_id IS 'Slots por profesional';
COMMENT ON INDEX idx_slots_prof_date IS 'Slots de un día específico';
COMMENT ON INDEX idx_slots_prof_date_available IS 'CRÍTICO: Slots disponibles para reservar';
COMMENT ON INDEX idx_slots_future_available IS 'Optimiza búsqueda de disponibilidad futura';

-- ============================================
-- ÍNDICES - slot_generation_config
-- ============================================

CREATE INDEX IF NOT EXISTS idx_slot_config_professional_id 
    ON slot_generation_config USING btree (professional_id);

CREATE INDEX IF NOT EXISTS idx_slot_config_active 
    ON slot_generation_config USING btree (is_active) 
    WHERE (is_active = true);

COMMENT ON INDEX idx_slot_config_professional_id IS 'Configuración por profesional';
COMMENT ON INDEX idx_slot_config_active IS 'Solo configuraciones activas';

-- ============================================
-- ÍNDICES ADICIONALES PARA REPORTES
-- ============================================

-- Índice para contar turnos por profesional y estado
CREATE INDEX IF NOT EXISTS idx_appointment_stats_prof_status 
    ON appointment USING btree (professional_id, status_id, appointment_date);

-- Índice para contar turnos por paciente
CREATE INDEX IF NOT EXISTS idx_appointment_stats_patient 
    ON appointment USING btree (patient_id, status_id, appointment_date);

-- Índice para búsquedas de turnos cancelados
CREATE INDEX IF NOT EXISTS idx_appointment_cancelled 
    ON appointment USING btree (cancelled_at DESC) 
    WHERE (cancelled_at IS NOT NULL);

COMMENT ON INDEX idx_appointment_stats_prof_status IS 'Estadísticas por profesional y estado';
COMMENT ON INDEX idx_appointment_stats_patient IS 'Historial completo del paciente';
COMMENT ON INDEX idx_appointment_cancelled IS 'Turnos cancelados para análisis';
