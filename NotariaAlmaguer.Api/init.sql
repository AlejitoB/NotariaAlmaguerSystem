PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS usuarios (
    id_usuario INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre TEXT NOT NULL,
    email TEXT NOT NULL UNIQUE,
    password TEXT NOT NULL,
    rol TEXT NOT NULL,
    fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS clientes (
    id_cliente INTEGER PRIMARY KEY AUTOINCREMENT,
    cedula TEXT NOT NULL UNIQUE,
    nombre TEXT NOT NULL,
    telefono TEXT,
    direccion TEXT,
    fecha_registro DATE DEFAULT CURRENT_DATE
);

CREATE TABLE IF NOT EXISTS notarios (
    id_notario INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre TEXT NOT NULL,
    numero_licencia TEXT NOT NULL UNIQUE,
    telefono TEXT,
    email TEXT,
    activo INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS citas (
    id_cita INTEGER PRIMARY KEY AUTOINCREMENT,
    id_cliente INTEGER NOT NULL,
    id_notario INTEGER NOT NULL,
    fecha_cita DATETIME NOT NULL,
    estado TEXT NOT NULL DEFAULT 'pendiente',
    descripcion TEXT,
    FOREIGN KEY (id_cliente) REFERENCES clientes(id_cliente) ON DELETE CASCADE,
    FOREIGN KEY (id_notario) REFERENCES notarios(id_notario) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS documentos (
    id_documento INTEGER PRIMARY KEY AUTOINCREMENT,
    id_cliente INTEGER NOT NULL,
    id_notario INTEGER NOT NULL,
    numero_documento TEXT NOT NULL,
    tipo_documento TEXT NOT NULL,
    descripcion TEXT,
    fecha_documento DATE DEFAULT CURRENT_DATE,
    FOREIGN KEY (id_cliente) REFERENCES clientes(id_cliente) ON DELETE CASCADE,
    FOREIGN KEY (id_notario) REFERENCES notarios(id_notario) ON DELETE CASCADE
);

-- Datos de prueba
INSERT OR IGNORE INTO notarios (nombre, numero_licencia, telefono, email, activo)
VALUES ('Dr. Carlos Muñoz', 'NOT-001', '3001234567', 'cmunoz@notaria.com', 1);

INSERT OR IGNORE INTO clientes (cedula, nombre, telefono, direccion)
VALUES ('12345678', 'Ana María López', '3109876543', 'Calle 5 #3-20, Almaguer');

INSERT OR IGNORE INTO clientes (cedula, nombre, telefono, direccion)
VALUES ('87654321', 'Jorge Hernández', '3201122334', 'Vereda El Rosal, Almaguer');
