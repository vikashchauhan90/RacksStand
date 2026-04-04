-- =====================================================
-- Flyway Migration: V1__Initial_Schema
-- Description: UserManagement module initial schema
-- Author: RacksStands Team
-- Date: 2026-04-04
-- =====================================================

-- Flyway automatically wraps in transaction, but we keep EF's transaction
-- START TRANSACTION is already in the generated script

CREATE DATABASE IF NOT EXISTS "RacksStands";
-- Create schema if not exists (modified for Flyway compatibility)
CREATE SCHEMA IF NOT EXISTS "UserManagement";

 CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT pk___ef_migrations_history PRIMARY KEY (migration_id)
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'UserManagement') THEN
            CREATE SCHEMA "UserManagement";
        END IF;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE TABLE "UserManagement".m2m_clients (
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE TABLE "UserManagement".magic_link_tokens (
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE TABLE "UserManagement".mfa_challenges (
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE TABLE "UserManagement".permissions (
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE TABLE "UserManagement".products (
        id character varying(36) NOT NULL,
        name character varying(150) NOT NULL,
        description character varying(500) NOT NULL,
        concurrency_stamp character varying(40),
        created_at timestamp with time zone NOT NULL,
        updated_at timestamp with time zone,
        deleted_at timestamp with time zone,
        CONSTRAINT pk_products PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE TABLE "UserManagement".refresh_tokens (
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE TABLE "UserManagement".role_permissions (
        role_id character varying(36) NOT NULL,
        permission_id character varying(36) NOT NULL,
        CONSTRAINT pk_role_permissions PRIMARY KEY (role_id, permission_id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE TABLE "UserManagement".roles (
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE TABLE "UserManagement".tenant_invitations (
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE TABLE "UserManagement".tenant_memberships (
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE TABLE "UserManagement".tenant_subscriptions (
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE TABLE "UserManagement".tenants (
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE TABLE "UserManagement".user_mfa_settings (
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE TABLE "UserManagement".users (
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE UNIQUE INDEX ix_m2m_clients_client_id ON "UserManagement".m2m_clients (client_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_m2m_clients_client_id_deleted_at ON "UserManagement".m2m_clients (client_id, deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_m2m_clients_deleted_at ON "UserManagement".m2m_clients (deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_m2m_clients_name ON "UserManagement".m2m_clients (name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_m2m_clients_scopes_deleted_at ON "UserManagement".m2m_clients (scopes, deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_magic_link_tokens_deleted_at ON "UserManagement".magic_link_tokens (deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_magic_link_tokens_email ON "UserManagement".magic_link_tokens (email);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_magic_link_tokens_expire_at ON "UserManagement".magic_link_tokens (expire_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_magic_link_tokens_is_revoked ON "UserManagement".magic_link_tokens (is_revoked);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_magic_link_tokens_is_used ON "UserManagement".magic_link_tokens (is_used);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE UNIQUE INDEX ix_magic_link_tokens_token_hash ON "UserManagement".magic_link_tokens (token_hash);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_magic_link_tokens_user_id ON "UserManagement".magic_link_tokens (user_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_magic_link_tokens_user_id_email_is_used_is_revoked_deleted_ ON "UserManagement".magic_link_tokens (user_id, email, is_used, is_revoked, deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_mfa_challenges_deleted_at ON "UserManagement".mfa_challenges (deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_mfa_challenges_expire_at ON "UserManagement".mfa_challenges (expire_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_mfa_challenges_is_used ON "UserManagement".mfa_challenges (is_used);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE UNIQUE INDEX ix_mfa_challenges_token_hash ON "UserManagement".mfa_challenges (token_hash);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_mfa_challenges_user_id ON "UserManagement".mfa_challenges (user_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_mfa_challenges_user_id_is_used_deleted_at ON "UserManagement".mfa_challenges (user_id, is_used, deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_permissions_deleted_at ON "UserManagement".permissions (deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_permissions_group ON "UserManagement".permissions ("group");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_permissions_is_system ON "UserManagement".permissions (is_system);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_permissions_is_system_deleted_at ON "UserManagement".permissions (is_system, deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_permissions_name ON "UserManagement".permissions (name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_permissions_tenant_id ON "UserManagement".permissions (tenant_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE UNIQUE INDEX ix_permissions_tenant_id_name ON "UserManagement".permissions (tenant_id, name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_products_deleted_at ON "UserManagement".products (deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE UNIQUE INDEX ix_products_name ON "UserManagement".products (name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_products_name_deleted_at ON "UserManagement".products (name, deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_refresh_tokens_deleted_at ON "UserManagement".refresh_tokens (deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_refresh_tokens_expire_at ON "UserManagement".refresh_tokens (expire_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_refresh_tokens_is_revoked ON "UserManagement".refresh_tokens (is_revoked);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE UNIQUE INDEX ix_refresh_tokens_token_hash ON "UserManagement".refresh_tokens (token_hash);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_refresh_tokens_user_id ON "UserManagement".refresh_tokens (user_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_refresh_tokens_user_id_is_revoked_deleted_at ON "UserManagement".refresh_tokens (user_id, is_revoked, deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_role_permissions_permission_id ON "UserManagement".role_permissions (permission_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_role_permissions_role_id ON "UserManagement".role_permissions (role_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE UNIQUE INDEX ix_role_permissions_role_id_permission_id ON "UserManagement".role_permissions (role_id, permission_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_roles_deleted_at ON "UserManagement".roles (deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_roles_is_system ON "UserManagement".roles (is_system);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_roles_is_system_deleted_at ON "UserManagement".roles (is_system, deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_roles_name ON "UserManagement".roles (name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_roles_tenant_id ON "UserManagement".roles (tenant_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE UNIQUE INDEX ix_roles_tenant_id_name ON "UserManagement".roles (tenant_id, name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_invitations_accepted_by_user_id ON "UserManagement".tenant_invitations (accepted_by_user_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_invitations_deleted_at ON "UserManagement".tenant_invitations (deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_invitations_email ON "UserManagement".tenant_invitations (email);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_invitations_expire_at ON "UserManagement".tenant_invitations (expire_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_invitations_responded_at ON "UserManagement".tenant_invitations (responded_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_invitations_revoked_at ON "UserManagement".tenant_invitations (revoked_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_invitations_role_id ON "UserManagement".tenant_invitations (role_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_invitations_status ON "UserManagement".tenant_invitations (status);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_invitations_tenant_id ON "UserManagement".tenant_invitations (tenant_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_invitations_tenant_id_email_status_deleted_at ON "UserManagement".tenant_invitations (tenant_id, email, status, deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_invitations_tenant_subscription_id ON "UserManagement".tenant_invitations (tenant_subscription_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE UNIQUE INDEX ix_tenant_invitations_token_hash ON "UserManagement".tenant_invitations (token_hash);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_memberships_assigned_by_user_id ON "UserManagement".tenant_memberships (assigned_by_user_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_memberships_deleted_at ON "UserManagement".tenant_memberships (deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_memberships_joined_at ON "UserManagement".tenant_memberships (joined_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_memberships_revoked_at ON "UserManagement".tenant_memberships (revoked_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_memberships_role_id ON "UserManagement".tenant_memberships (role_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_memberships_role_id_deleted_at ON "UserManagement".tenant_memberships (role_id, deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_memberships_tenant_id ON "UserManagement".tenant_memberships (tenant_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE UNIQUE INDEX ix_tenant_memberships_tenant_id_user_id_deleted_at ON "UserManagement".tenant_memberships (tenant_id, user_id, deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_memberships_user_id ON "UserManagement".tenant_memberships (user_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_subscriptions_deleted_at ON "UserManagement".tenant_subscriptions (deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_subscriptions_expire_at ON "UserManagement".tenant_subscriptions (expire_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_subscriptions_plan ON "UserManagement".tenant_subscriptions (plan);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_subscriptions_plan_deleted_at ON "UserManagement".tenant_subscriptions (plan, deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_subscriptions_product_id ON "UserManagement".tenant_subscriptions (product_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_subscriptions_started_at ON "UserManagement".tenant_subscriptions (started_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenant_subscriptions_tenant_id ON "UserManagement".tenant_subscriptions (tenant_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE UNIQUE INDEX ix_tenant_subscriptions_tenant_id_product_id_deleted_at ON "UserManagement".tenant_subscriptions (tenant_id, product_id, deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenants_deleted_at ON "UserManagement".tenants (deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenants_name ON "UserManagement".tenants (name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenants_owner_id ON "UserManagement".tenants (owner_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE UNIQUE INDEX ix_tenants_slug ON "UserManagement".tenants (slug);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_tenants_slug_deleted_at ON "UserManagement".tenants (slug, deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_user_mfa_settings_deleted_at ON "UserManagement".user_mfa_settings (deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_user_mfa_settings_is_enabled ON "UserManagement".user_mfa_settings (is_enabled);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_user_mfa_settings_last_used_at ON "UserManagement".user_mfa_settings (last_used_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_user_mfa_settings_user_id ON "UserManagement".user_mfa_settings (user_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_user_mfa_settings_user_id_is_enabled_deleted_at ON "UserManagement".user_mfa_settings (user_id, is_enabled, deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_users_created_at ON "UserManagement".users (created_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_users_deleted_at ON "UserManagement".users (deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE UNIQUE INDEX ix_users_email ON "UserManagement".users (email);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE INDEX ix_users_email_deleted_at ON "UserManagement".users (email, deleted_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    CREATE UNIQUE INDEX ix_users_user_name ON "UserManagement".users (user_name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260404111958_InitialCreate_Complete') THEN
    INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20260404111958_InitialCreate_Complete', '10.0.1');
    END IF;
END $EF$;
COMMIT;

