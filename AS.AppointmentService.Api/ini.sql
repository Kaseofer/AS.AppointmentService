-- INSTALAR LA EXTENSIÓN PRIMERO
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- ========================================
-- TABLA CON UUIDs (RECOMENDADA PARA MICROSERVICIOS)
-- ========================================

CREATE TABLE IF NOT EXISTS agenda_citas
(
    id uuid NOT NULL DEFAULT uuid_generate_v4(),
    profesional_id uuid NOT NULL,
    fecha date NOT NULL,
    hora_inicio time without time zone NOT NULL,
    hora_fin time without time zone NOT NULL,
    ocupado boolean DEFAULT false,
    paciente_id uuid,
    estado_cita_id uuid,
    motivo_cita_id uuid,
    observaciones character varying(255),
    usuario_id uuid,
    vencida boolean DEFAULT false,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT agenda_citas_pkey PRIMARY KEY (id)
);

-- ÍNDICES
CREATE INDEX IF NOT EXISTS idx_agenda_citas_profesional_id
    ON agenda_citas USING btree (profesional_id);

CREATE INDEX IF NOT EXISTS idx_agenda_citas_profesional_fecha
    ON agenda_citas USING btree (profesional_id, fecha);

CREATE INDEX IF NOT EXISTS idx_agenda_citas_paciente_id
    ON agenda_citas USING btree (paciente_id);

CREATE INDEX IF NOT EXISTS idx_agenda_citas_disponibles
    ON agenda_citas USING btree (profesional_id, fecha, ocupado)
    WHERE ocupado = false;

-- PERMISOS
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE agenda_citas TO api_appointment;
ALTER TABLE agenda_citas OWNER TO neondb_owner;


-- ========================================
-- CREAR SECUENCIAS NECESARIAS
-- ========================================

-- Secuencia para estado_cita
CREATE SEQUENCE IF NOT EXISTS estado_cita_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

-- ========================================
-- TABLA: estado_cita
-- ========================================

CREATE TABLE IF NOT EXISTS estado_cita
(
    id integer NOT NULL DEFAULT nextval('estado_cita_id_seq'::regclass),
    nombre character varying(50) NOT NULL,
    descripcion text,
    color_hex character(7) DEFAULT '#CCCCCC',
    activo boolean DEFAULT true,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT estado_cita_pkey PRIMARY KEY (id),
    CONSTRAINT estado_cita_nombre_unique UNIQUE (nombre),
    CONSTRAINT estado_cita_color_hex_check CHECK (color_hex ~* '^#[0-9A-F]{6}$')
);

-- ========================================
-- ÍNDICES PARA PERFORMANCE
-- ========================================

-- Índice para consultas de estados activos
CREATE INDEX IF NOT EXISTS idx_estado_cita_activo
    ON estado_cita USING btree (activo)
    WHERE activo = true;

-- Índice por nombre para búsquedas
CREATE INDEX IF NOT EXISTS idx_estado_cita_nombre
    ON estado_cita USING btree (nombre);

-- ========================================
-- DATOS INICIALES (ESTADOS TÍPICOS)
-- ========================================

INSERT INTO estado_cita (nombre, descripcion, color_hex) VALUES
    ('Programado', 'Cita programada y confirmada', '#4CAF50'),
    ('Pendiente', 'Cita pendiente de confirmación', '#FF9800'),
    ('Cancelado', 'Cita cancelada por el paciente', '#F44336'),
    ('No Asistió', 'Paciente no se presentó a la cita', '#9E9E9E'),
    ('Completado', 'Cita realizada exitosamente', '#2196F3'),
    ('Reagendado', 'Cita movida a otra fecha/hora', '#9C27B0')
ON CONFLICT (nombre) DO NOTHING;

-- ========================================
-- PERMISOS PARA EL USUARIO api_appointment
-- ========================================

-- Tabla
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE estado_cita TO api_appointment;

-- Secuencia
GRANT USAGE, SELECT ON SEQUENCE estado_cita_id_seq TO api_appointment;

-- Owner
ALTER TABLE estado_cita OWNER TO neondb_owner;
ALTER SEQUENCE estado_cita_id_seq OWNER TO neondb_owner;


-- ========================================
-- CREAR SECUENCIAS NECESARIAS
-- ========================================

-- Secuencia para motivo_cita
CREATE SEQUENCE IF NOT EXISTS motivo_cita_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

-- ========================================
-- TABLA: motivo_cita
-- ========================================

CREATE TABLE IF NOT EXISTS motivo_cita
(
    id integer NOT NULL DEFAULT nextval('motivo_cita_id_seq'::regclass),
    nombre character varying(100) NOT NULL,
    descripcion text,
    activo boolean DEFAULT true,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT motivo_cita_pkey PRIMARY KEY (id),
    CONSTRAINT motivo_cita_nombre_unique UNIQUE (nombre)
);

-- ========================================
-- ÍNDICES PARA PERFORMANCE
-- ========================================

-- Índice para motivos activos
CREATE INDEX IF NOT EXISTS idx_motivo_cita_activo
    ON motivo_cita USING btree (activo)
    WHERE activo = true;

-- Índice por nombre para búsquedas
CREATE INDEX IF NOT EXISTS idx_motivo_cita_nombre
    ON motivo_cita USING btree (nombre);

-- ========================================
-- DATOS INICIALES (MOTIVOS TÍPICOS MÉDICOS)
-- ========================================

INSERT INTO motivo_cita (nombre, descripcion) VALUES
    ('Consulta General', 'Consulta médica de rutina o chequeo general'),
    ('Control Post-Operatorio', 'Revisión médica posterior a una cirugía'),
    ('Emergencia', 'Atención médica urgente'),
    ('Vacunación', 'Aplicación de vacunas'),
    ('Chequeo Anual', 'Control médico preventivo anual'),
    ('Seguimiento Tratamiento', 'Control de evolución de tratamiento médico'),
    ('Segunda Opinión', 'Consulta para obtener una segunda opinión médica'),
    ('Certificado Médico', 'Emisión de certificados o constancias médicas'),
    ('Consulta Especializada', 'Atención por especialista médico'),
    ('Control Prenatal', 'Control médico durante el embarazo')
ON CONFLICT (nombre) DO NOTHING;

-- ========================================
-- PERMISOS PARA EL USUARIO api_appointment
-- ========================================

-- Tabla
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE motivo_cita TO api_appointment;

-- Secuencia
GRANT USAGE, SELECT ON SEQUENCE motivo_cita_id_seq TO api_appointment;

-- Owner
ALTER TABLE motivo_cita OWNER TO neondb_owner;
ALTER SEQUENCE motivo_cita_id_seq OWNER TO neondb_owner;