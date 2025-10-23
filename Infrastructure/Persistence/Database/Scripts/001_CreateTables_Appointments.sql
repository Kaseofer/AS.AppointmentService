-- ============================================
-- CREACIÓN DE TABLAS - MICROSERVICIO APPOINTMENTS
-- ============================================
-- Proyecto: Sistema de Gestión de Turnos Médicos
-- Microservicio: Appointments
-- Base de Datos: PostgreSQL 17
-- Fecha: 2025-10-23
-- ============================================

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

-- ============================================
-- EXTENSIONES
-- ============================================

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- ============================================
-- TABLA: appointment_status
-- Descripción: Estados posibles de un turno
-- ============================================

CREATE TABLE IF NOT EXISTS appointment_status (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    description TEXT,
    color VARCHAR(7),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

COMMENT ON TABLE appointment_status IS 'Estados de los turnos médicos';
COMMENT ON COLUMN appointment_status.name IS 'Nombre del estado: Programado, Cancelado, Completado, etc.';
COMMENT ON COLUMN appointment_status.color IS 'Color hex para UI: #28a745, #dc3545, etc.';

-- ============================================
-- TABLA: appointment_reason
-- Descripción: Motivos/razones de consulta
-- ============================================

CREATE TABLE IF NOT EXISTS appointment_reason (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    description TEXT,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

COMMENT ON TABLE appointment_reason IS 'Motivos de consulta médica';
COMMENT ON COLUMN appointment_reason.name IS 'Ej: Consulta general, Control, Emergencia, etc.';

-- ============================================
-- TABLA: appointment
-- Descripción: Turnos médicos programados
-- ============================================

CREATE TABLE IF NOT EXISTS appointment (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    professional_id UUID NOT NULL,
    patient_id UUID NOT NULL,
    status_id INTEGER NOT NULL,
    reason_id INTEGER,
    appointment_date DATE NOT NULL,
    start_time TIME WITHOUT TIME ZONE NOT NULL,
    end_time TIME WITHOUT TIME ZONE NOT NULL,
    duration_minutes INTEGER NOT NULL,
    notes TEXT,
    cancellation_reason TEXT,
    created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITHOUT TIME ZONE,
    created_by UUID,
    cancelled_at TIMESTAMP WITHOUT TIME ZONE,
    cancelled_by UUID
);

COMMENT ON TABLE appointment IS 'Turnos médicos programados';
COMMENT ON COLUMN appointment.professional_id IS 'UUID del profesional (FK a microservicio Medical)';
COMMENT ON COLUMN appointment.patient_id IS 'UUID del paciente (FK a microservicio Medical)';
COMMENT ON COLUMN appointment.duration_minutes IS 'Duración en minutos: 10, 15, 20, 30, 45, 60';
COMMENT ON COLUMN appointment.notes IS 'Notas adicionales del turno';
COMMENT ON COLUMN appointment.cancellation_reason IS 'Motivo de cancelación si aplica';

-- ============================================
-- TABLA: national_holidays
-- Descripción: Feriados nacionales
-- ============================================

CREATE TABLE IF NOT EXISTS national_holidays (
    id SERIAL PRIMARY KEY,
    holiday_date DATE NOT NULL UNIQUE,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    is_movable BOOLEAN DEFAULT false,
    created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

COMMENT ON TABLE national_holidays IS 'Feriados nacionales de Argentina';
COMMENT ON COLUMN national_holidays.is_movable IS 'true = feriado inamovible, false = feriado trasladable';

-- ============================================
-- TABLA: professional_non_working_days
-- Descripción: Días no laborables del profesional
-- ============================================

CREATE TABLE IF NOT EXISTS professional_non_working_days (
    id SERIAL PRIMARY KEY,
    professional_id UUID NOT NULL,
    non_working_date DATE NOT NULL,
    reason VARCHAR(100),
    is_recurring BOOLEAN DEFAULT false,
    created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

COMMENT ON TABLE professional_non_working_days IS 'Días en que el profesional no atiende (vacaciones, licencias, etc.)';
COMMENT ON COLUMN professional_non_working_days.is_recurring IS 'true = se repite anualmente (ej: cumpleaños)';

-- ============================================
-- TABLA: professional_schedule
-- Descripción: Horario semanal del profesional
-- ============================================

CREATE TABLE IF NOT EXISTS professional_schedule (
    id SERIAL PRIMARY KEY,
    professional_id UUID NOT NULL,
    day_of_week INTEGER NOT NULL,
    start_time TIME WITHOUT TIME ZONE NOT NULL,
    end_time TIME WITHOUT TIME ZONE NOT NULL,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITHOUT TIME ZONE,
    CONSTRAINT check_day_of_week CHECK (day_of_week >= 0 AND day_of_week <= 6),
    CONSTRAINT check_time_range CHECK (end_time > start_time)
);

COMMENT ON TABLE professional_schedule IS 'Horario de atención semanal del profesional';
COMMENT ON COLUMN professional_schedule.day_of_week IS '0=Domingo, 1=Lunes, 2=Martes, 3=Miércoles, 4=Jueves, 5=Viernes, 6=Sábado';

-- ============================================
-- TABLA: appointment_slots
-- Descripción: Slots de tiempo disponibles generados automáticamente
-- ============================================

CREATE TABLE IF NOT EXISTS appointment_slots (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    professional_id UUID NOT NULL,
    slot_date DATE NOT NULL,
    start_time TIME WITHOUT TIME ZONE NOT NULL,
    end_time TIME WITHOUT TIME ZONE NOT NULL,
    duration_minutes INTEGER NOT NULL,
    is_available BOOLEAN DEFAULT true,
    appointment_id UUID,
    created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITHOUT TIME ZONE
);

COMMENT ON TABLE appointment_slots IS 'Slots de tiempo disponibles para turnos';
COMMENT ON COLUMN appointment_slots.is_available IS 'true = disponible, false = ocupado';
COMMENT ON COLUMN appointment_slots.appointment_id IS 'UUID del turno si está ocupado';

-- ============================================
-- TABLA: slot_generation_config
-- Descripción: Configuración para generación automática de slots
-- ============================================

CREATE TABLE IF NOT EXISTS slot_generation_config (
    id SERIAL PRIMARY KEY,
    professional_id UUID NOT NULL UNIQUE,
    default_duration_minutes INTEGER NOT NULL DEFAULT 30,
    advance_booking_days INTEGER NOT NULL DEFAULT 30,
    same_day_booking_allowed BOOLEAN DEFAULT true,
    buffer_minutes INTEGER DEFAULT 0,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITHOUT TIME ZONE,
    CONSTRAINT check_duration CHECK (default_duration_minutes IN (10, 15, 20, 30, 45, 60)),
    CONSTRAINT check_advance_days CHECK (advance_booking_days > 0 AND advance_booking_days <= 365)
);

COMMENT ON TABLE slot_generation_config IS 'Configuración para generación automática de slots por profesional';
COMMENT ON COLUMN slot_generation_config.default_duration_minutes IS 'Duración por defecto de cada turno: 10, 15, 20, 30, 45 o 60 minutos';
COMMENT ON COLUMN slot_generation_config.advance_booking_days IS 'Cuántos días adelante se pueden sacar turnos (1-365)';
COMMENT ON COLUMN slot_generation_config.same_day_booking_allowed IS 'Permitir sacar turnos el mismo día';
COMMENT ON COLUMN slot_generation_config.buffer_minutes IS 'Minutos de buffer entre turnos (0, 5, 10, 15)';
