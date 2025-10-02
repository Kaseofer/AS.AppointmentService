-- =====================================================
-- TABLA: slot_generation_config
-- Configuraci�n de generaci�n de slots por profesional
-- =====================================================

CREATE TABLE IF NOT EXISTS slot_generation_config (
    id SERIAL PRIMARY KEY,
    professional_id UUID NOT NULL UNIQUE,
    advance_booking_days INTEGER DEFAULT 60,
    min_advance_hours INTEGER DEFAULT 24,
    allow_same_day_booking BOOLEAN DEFAULT false,
    auto_generate_slots BOOLEAN DEFAULT true,
    max_appointments_per_day INTEGER,
    buffer_time_mins INTEGER DEFAULT 0,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW(),
    
    CONSTRAINT fk_slot_generation_config_professional 
        FOREIGN KEY (professional_id) 
        REFERENCES professional(id) 
        ON DELETE CASCADE,
    
    CONSTRAINT check_advance_booking_days 
        CHECK (advance_booking_days >= 1 AND advance_booking_days <= 365),
    
    CONSTRAINT check_min_advance_hours 
        CHECK (min_advance_hours >= 0 AND min_advance_hours <= 168),
    
    CONSTRAINT check_max_appointments 
        CHECK (max_appointments_per_day IS NULL OR max_appointments_per_day > 0),
    
    CONSTRAINT check_buffer_time 
        CHECK (buffer_time_mins >= 0 AND buffer_time_mins <= 60)
);

-- �ndices
CREATE INDEX idx_slot_generation_config_professional_id 
ON slot_generation_config(professional_id);

CREATE INDEX idx_slot_generation_config_auto_generate 
ON slot_generation_config(auto_generate_slots) 
WHERE auto_generate_slots = true;

-- Comentarios
COMMENT ON TABLE slot_generation_config IS 'Configuraci�n de generaci�n de slots por profesional';
COMMENT ON COLUMN slot_generation_config.advance_booking_days IS 'Cantidad de d�as hacia adelante que se pueden reservar turnos (por defecto 60 = 2 meses)';
COMMENT ON COLUMN slot_generation_config.min_advance_hours IS 'M�nimo de horas de anticipaci�n para reservar (por defecto 24)';
COMMENT ON COLUMN slot_generation_config.allow_same_day_booking IS 'Permite reservar turnos el mismo d�a';
COMMENT ON COLUMN slot_generation_config.auto_generate_slots IS 'Genera autom�ticamente slots o se hace manualmente';
COMMENT ON COLUMN slot_generation_config.max_appointments_per_day IS 'L�mite de turnos por d�a (NULL = sin l�mite)';
COMMENT ON COLUMN slot_generation_config.buffer_time_mins IS 'Tiempo de descanso entre turnos en minutos';

-- Permisos
ALTER TABLE slot_generation_config OWNER TO neondb_owner;
GRANT INSERT, DELETE, SELECT, UPDATE ON TABLE slot_generation_config TO api_user;
GRANT USAGE, SELECT ON SEQUENCE slot_generation_config_id_seq TO api_user;

-- Insertar configuraci�n por defecto para profesionales existentes
INSERT INTO slot_generation_config (professional_id)
SELECT id FROM professional
ON CONFLICT (professional_id) DO NOTHING;