-- =====================================================
-- Flyway Migration: V1__Initial_Schema
-- Description: UserManagement module initial schema
-- Author: RacksStands Team
-- =====================================================

-- Create schema if not exists
CREATE SCHEMA IF NOT EXISTS "UserManagement";

-- Set search path
SET search_path TO "UserManagement", public;

-- =====================================================
-- EF Core Migrations History Table
-- =====================================================
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT pk___ef_migrations_history PRIMARY KEY (migration_id)
);

-- =====================================================
-- Tables (using IF NOT EXISTS - Flyway friendly)
-- =====================================================

-- Independent tables
CREATE TABLE IF NOT EXISTS m2m_clients (
    id character varying(36) NOT NULL,
    name character varying(100) NOT NULL,
    client_id character varying(100) NOT NULL,
    client_secret_hash text NOT NULL,
    scopes character varying(500) NOT NULL,
    concurrency_stamp character varying(40),
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    deleted_at timestamp with time zone,
    CONSTRAINT pk_m2m_clients PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS products (
    id character varying(36) NOT NULL,
    name character varying(150) NOT NULL,
    description character varying(500) NOT NULL,
    concurrency_stamp character varying(40),
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    deleted_at timestamp with time zone,
    CONSTRAINT pk_products PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS tenants (
    id character varying(36) NOT NULL,
    name character varying(150) NOT NULL,
    slug character varying(100) NOT NULL,
    owner_id character varying(36) NOT NULL,
    concurrency_stamp character varying(40),
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    deleted_at timestamp with time zone,
    CONSTRAINT pk_tenants PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS users (
    id character varying(36) NOT NULL,
    name character varying(100) NOT NULL,
    user_name character varying(50) NOT NULL,
    email character varying(256) NOT NULL,
    password_hash text NOT NULL,
    concurrency_stamp character varying(40),
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    deleted_at timestamp with time zone,
    CONSTRAINT pk_users PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS permissions (
    id character varying(36) NOT NULL,
    name character varying(100) NOT NULL,
    description character varying(250) NOT NULL,
    "group" character varying(100) NOT NULL,
    tenant_id character varying(36) NOT NULL,
    is_system boolean NOT NULL,
    concurrency_stamp character varying(40),
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    deleted_at timestamp with time zone,
    CONSTRAINT pk_permissions PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS roles (
    id character varying(36) NOT NULL,
    name character varying(100) NOT NULL,
    description character varying(250) NOT NULL,
    tenant_id character varying(36) NOT NULL,
    is_system boolean NOT NULL,
    concurrency_stamp character varying(40),
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    deleted_at timestamp with time zone,
    CONSTRAINT pk_roles PRIMARY KEY (id)
);

-- Junction table
CREATE TABLE IF NOT EXISTS role_permissions (
    role_id character varying(36) NOT NULL,
    permission_id character varying(36) NOT NULL,
    CONSTRAINT pk_role_permissions PRIMARY KEY (role_id, permission_id)
);

-- Dependent tables
CREATE TABLE IF NOT EXISTS magic_link_tokens (
    id character varying(36) NOT NULL,
    user_id character varying(36) NOT NULL,
    email character varying(100) NOT NULL,
    token_hash character varying(256) NOT NULL,
    is_used boolean NOT NULL,
    is_revoked boolean NOT NULL,
    expire_at timestamp with time zone NOT NULL,
    used_at timestamp with time zone,
    concurrency_stamp character varying(40),
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    deleted_at timestamp with time zone,
    CONSTRAINT pk_magic_link_tokens PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS mfa_challenges (
    id character varying(36) NOT NULL,
    user_id character varying(36) NOT NULL,
    token_hash character varying(256) NOT NULL,
    is_used boolean NOT NULL,
    expire_at timestamp with time zone NOT NULL,
    used_at timestamp with time zone,
    concurrency_stamp character varying(40),
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    deleted_at timestamp with time zone,
    CONSTRAINT pk_mfa_challenges PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS refresh_tokens (
    id character varying(36) NOT NULL,
    user_id character varying(36) NOT NULL,
    token_hash character varying(256) NOT NULL,
    is_revoked boolean NOT NULL,
    expire_at timestamp with time zone NOT NULL,
    concurrency_stamp character varying(40),
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    deleted_at timestamp with time zone,
    CONSTRAINT pk_refresh_tokens PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS tenant_subscriptions (
    id character varying(36) NOT NULL,
    tenant_id character varying(36) NOT NULL,
    product_id character varying(36) NOT NULL,
    plan integer NOT NULL,
    started_at timestamp with time zone NOT NULL,
    expire_at timestamp with time zone,
    concurrency_stamp character varying(40),
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    deleted_at timestamp with time zone,
    CONSTRAINT pk_tenant_subscriptions PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS tenant_memberships (
    id character varying(36) NOT NULL,
    tenant_id character varying(36) NOT NULL,
    user_id character varying(36) NOT NULL,
    role_id character varying(36) NOT NULL,
    assigned_by_user_id character varying(36),
    joined_at timestamp with time zone NOT NULL,
    revoked_at timestamp with time zone,
    concurrency_stamp character varying(40),
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    deleted_at timestamp with time zone,
    CONSTRAINT pk_tenant_memberships PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS tenant_invitations (
    id character varying(36) NOT NULL,
    tenant_id character varying(36) NOT NULL,
    tenant_subscription_id character varying(36) NOT NULL,
    role_id character varying(36) NOT NULL,
    email character varying(100) NOT NULL,
    token_hash character varying(256) NOT NULL,
    invited_by_user_id character varying(36),
    status integer NOT NULL,
    expire_at timestamp with time zone,
    revoked_at timestamp with time zone,
    accepted_by_user_id character varying(36),
    responded_at timestamp with time zone,
    concurrency_stamp character varying(40),
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    deleted_at timestamp with time zone,
    CONSTRAINT pk_tenant_invitations PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS user_mfa_settings (
    id character varying(36) NOT NULL,
    user_id character varying(36) NOT NULL,
    recovery_code_hash text NOT NULL,
    totp_secret_encrypted text NOT NULL,
    is_enabled boolean NOT NULL,
    last_used_at timestamp with time zone,
    concurrency_stamp character varying(40),
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    deleted_at timestamp with time zone,
    CONSTRAINT pk_user_mfa_settings PRIMARY KEY (id)
);

-- =====================================================
-- Indexes
-- =====================================================
CREATE INDEX IF NOT EXISTS ix_m2m_clients_client_id ON m2m_clients (client_id);
CREATE INDEX IF NOT EXISTS ix_m2m_clients_deleted_at ON m2m_clients (deleted_at);
CREATE INDEX IF NOT EXISTS ix_m2m_clients_name ON m2m_clients (name);
CREATE INDEX IF NOT EXISTS ix_products_deleted_at ON products (deleted_at);
CREATE UNIQUE INDEX IF NOT EXISTS ix_products_name ON products (name);
CREATE INDEX IF NOT EXISTS ix_tenants_deleted_at ON tenants (deleted_at);
CREATE UNIQUE INDEX IF NOT EXISTS ix_tenants_slug ON tenants (slug);
CREATE INDEX IF NOT EXISTS ix_users_deleted_at ON users (deleted_at);
CREATE UNIQUE INDEX IF NOT EXISTS ix_users_email ON users (email);
CREATE UNIQUE INDEX IF NOT EXISTS ix_users_user_name ON users (user_name);
CREATE INDEX IF NOT EXISTS ix_permissions_deleted_at ON permissions (deleted_at);
CREATE UNIQUE INDEX IF NOT EXISTS ix_permissions_tenant_id_name ON permissions (tenant_id, name);
CREATE INDEX IF NOT EXISTS ix_roles_deleted_at ON roles (deleted_at);
CREATE UNIQUE INDEX IF NOT EXISTS ix_roles_tenant_id_name ON roles (tenant_id, name);
CREATE INDEX IF NOT EXISTS ix_magic_link_tokens_user_id ON magic_link_tokens (user_id);
CREATE INDEX IF NOT EXISTS ix_magic_link_tokens_token_hash ON magic_link_tokens (token_hash);
CREATE INDEX IF NOT EXISTS ix_magic_link_tokens_expire_at ON magic_link_tokens (expire_at);
CREATE INDEX IF NOT EXISTS ix_mfa_challenges_user_id ON mfa_challenges (user_id);
CREATE INDEX IF NOT EXISTS ix_mfa_challenges_token_hash ON mfa_challenges (token_hash);
CREATE INDEX IF NOT EXISTS ix_mfa_challenges_expire_at ON mfa_challenges (expire_at);
CREATE INDEX IF NOT EXISTS ix_refresh_tokens_user_id ON refresh_tokens (user_id);
CREATE INDEX IF NOT EXISTS ix_refresh_tokens_token_hash ON refresh_tokens (token_hash);
CREATE INDEX IF NOT EXISTS ix_refresh_tokens_expire_at ON refresh_tokens (expire_at);
CREATE INDEX IF NOT EXISTS ix_tenant_subscriptions_deleted_at ON tenant_subscriptions (deleted_at);
CREATE INDEX IF NOT EXISTS ix_tenant_memberships_deleted_at ON tenant_memberships (deleted_at);
CREATE INDEX IF NOT EXISTS ix_tenant_memberships_tenant_id_user_id_deleted_at ON tenant_memberships (tenant_id, user_id, deleted_at);
CREATE INDEX IF NOT EXISTS ix_tenant_invitations_deleted_at ON tenant_invitations (deleted_at);
CREATE UNIQUE INDEX IF NOT EXISTS ix_tenant_invitations_token_hash ON tenant_invitations (token_hash);
CREATE INDEX IF NOT EXISTS ix_user_mfa_settings_deleted_at ON user_mfa_settings (deleted_at);
CREATE INDEX IF NOT EXISTS ix_user_mfa_settings_user_id ON user_mfa_settings (user_id);

-- =====================================================
-- Insert EF Core Migration Record (only if not exists)
-- =====================================================
INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
SELECT '20260404111958_InitialCreate_Complete', '10.0.1'
WHERE NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE migration_id = '20260404111958_InitialCreate_Complete');