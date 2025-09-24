-- ========================================
-- CREAR SECUENCIAS NECESARIAS
-- ========================================

-- Secuencia para agenda_citas (si quieres usar INTEGER en lugar de UUID)
CREATE SEQUENCE IF NOT EXISTS agenda_citas_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

-- ========================================
-- TABLA PRINCIPAL: agenda_citas
-- ========================================

CREATE TABLE IF NOT EXISTS agenda_citas
(
    id integer NOT NULL DEFAULT nextval('agenda_citas_id_seq'::regclass),
    profesional_id integer NOT NULL,
    fecha date NOT NULL,
    hora_inicio time without time zone NOT NULL,
    hora_fin time without time zone NOT NULL,
    ocupado boolean DEFAULT false,
    paciente_id integer,
    estado_cita_id integer,
    motivo_cita_id integer,
    observaciones character varying(255),
    usuario_id integer,
    vencida boolean DEFAULT false,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT agenda_citas_pkey PRIMARY KEY (id)
);

-- ========================================
-- ÍNDICES PARA PERFORMANCE
-- ========================================

-- Índice por profesional (consultas más frecuentes)
CREATE INDEX IF NOT EXISTS idx_agenda_citas_profesional_id
    ON agenda_citas USING btree (profesional_id ASC NULLS LAST);

-- Índice por fecha y profesional (para búsquedas por rango de fechas)
CREATE INDEX IF NOT EXISTS idx_agenda_citas_profesional_fecha
    ON agenda_citas USING btree (profesional_id ASC, fecha ASC);

-- Índice por paciente
CREATE INDEX IF NOT EXISTS idx_agenda_citas_paciente_id
    ON agenda_citas USING btree (paciente_id ASC NULLS LAST);

-- Índice para citas no ocupadas (consulta frecuente)
CREATE INDEX IF NOT EXISTS idx_agenda_citas_disponibles
    ON agenda_citas USING btree (profesional_id, fecha, ocupado)
    WHERE ocupado = false;

-- ========================================
-- PERMISOS PARA EL USUARIO api_appointment
-- ========================================

-- Tabla
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE agenda_citas TO api_appointment;

-- Secuencia
GRANT USAGE, SELECT ON SEQUENCE agenda_citas_id_seq TO api_appointment;

-- Owner
ALTER TABLE agenda_citas OWNER TO neondb_owner;
ALTER SEQUENCE agenda_citas_id_seq OWNER TO neondb_owner;