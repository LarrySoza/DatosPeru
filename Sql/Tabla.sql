CREATE DATABASE "datos_peru"
  WITH ENCODING='WIN1252'
       CONNECTION LIMIT=-1
       TEMPLATE template0;

CREATE EXTENSION "uuid-ossp";
CREATE EXTENSION "pgcrypto";

CREATE TABLE roles
(
	id text NOT NULL,
	nombre text NOT NULL,
	CONSTRAINT roles_pk PRIMARY KEY(id)
);

CREATE TABLE usuarios
(
	id text NOT NULL DEFAULT uuid_generate_v4(),
	fecha_registro timestamp with time zone NOT NULL DEFAULT now(),--fecha interna del sistema
        nombre_usuario text NOT NULL,--podria colocar aqui un nombre del usuario o su correo
        clave_hash text NOT NULL,--clas encriptada en sha1
        correo_confirmado boolean NOT NULL,
        CONSTRAINT usuario_pk PRIMARY KEY(id)
);

CREATE TABLE usuarios_roles
(
	usuario_id text NOT NULL,
	rol_id text NOT NULL,
	CONSTRAINT usuarios_roles_pk PRIMARY KEY(usuario_id, rol_id),
	CONSTRAINT usuarios_fkey FOREIGN KEY(usuario_id)
		REFERENCES usuarios(id) MATCH SIMPLE
		ON UPDATE RESTRICT ON DELETE RESTRICT,
	CONSTRAINT roles_fkey FOREIGN KEY(rol_id)
		REFERENCES roles(id) MATCH SIMPLE
		ON UPDATE RESTRICT ON DELETE RESTRICT
);

CREATE TABLE sunat_rucs
(
	ruc text NOT NULL,
	nombre_o_razon_social text,
	estado_del_contribuyente text,
	condicion_de_domicilio text,
	ubigeo text,
	tipo_de_via text,
	nombre_de_via text,
	codigo_de_zona text,
	tipo_de_zona text,
	numero text,
	interior text,
	lote text,
	dpto text,
	manzana text,
	kilometro text,
	adicional text,--ultima columna para que funcione el copy
	CONSTRAINT sunat_rucs_pk PRIMARY KEY(ruc)
);

CREATE TABLE sunat_anexos
(
	ruc text NOT NULL,
	ubigeo text,
	tipo_de_via text,
	nombre_de_via text,
	codigo_de_zona text,
	tipo_de_zona text,
	numero text,
	kilometro text,
	interior text,
	lote text,
	dpto text,
	manzana text,
	adicional text--ultima columna para que funcione el copy
);

CREATE TABLE sunat_tipo_cambio
(
	anio integer NOT NULL,
	mes integer NOT NULL,
	dia integer NOT NULL,
	venta numeric(11,3) NOT NULL,
	compra numeric(11,3) NOT NULL,
	CONSTRAINT sunat_tipo_cambio_pk PRIMARY KEY(anio, mes, dia)
);

CREATE TABLE reniec_dnis
(
	dni text NOT NULL,
	apellido_paterno text NOT NULL,
	apellido_materno text,
	nombres text NOT NULL,
	CONSTRAINT reniec_dnis_pk PRIMARY KEY(dni)
);

--Registra las fechas en las que se actuliza los datos desde sunat
CREATE TABLE sunat_fechas_actualizacion
(
	id text NOT NULL DEFAULT uuid_generate_v4(),
	fecha_hora timestamp with time zone NOT NULL,
        CONSTRAINT sunat_fechas_actualizacion_pk PRIMARY KEY(id)
);

INSERT INTO roles VALUES('U','Usuario');
INSERT INTO roles VALUES('A','Administrador');

INSERT INTO usuarios(id,nombre_usuario,clave_hash,correo_confirmado)VALUES
		    ('2b356c9c-e243-43c2-af1d-93e60d5eb159','admin',encode(digest('12345', 'sha1'), 'hex'),true);

INSERT INTO usuarios_roles VALUES('2b356c9c-e243-43c2-af1d-93e60d5eb159','U');
INSERT INTO usuarios_roles VALUES('2b356c9c-e243-43c2-af1d-93e60d5eb159','A');

copy sunat_rucs from E'C:\\webs\\Sunat\\padron_reducido_ruc.txt' with DELIMITER AS '|' ENCODING 'WIN1252';
copy sunat_anexos from E'C:\\webs\\Sunat\\padron_reducido_local_anexo.txt' with DELIMITER AS '|' ENCODING 'WIN1252';

DELETE FROM sunat_rucs WHERE ruc='RUC';
DELETE FROM sunat_anexos WHERE ubigeo='UBIGEO';

INSERT INTO sunat_fechas_actualizacion(fecha_hora) VALUES(now());