-- =====================================================
-- Flyway Undo: U1__Rollback
-- Description: Rollback UserManagement module V1 schema
-- Author: RacksStands Team
-- Date: 2026-04-04
-- WARNING: This will DELETE ALL DATA in UserManagement schema
-- =====================================================

-- Disable foreign key checks temporarily (PostgreSQL)
SET session_replication_role = 'replica';

-- Drop tables in reverse order of creation (to respect foreign keys)
DROP TABLE IF EXISTS "UserManagement"."__EFMigrationsHistory";
DROP TABLE IF EXISTS "UserManagement"."tenant_invitations";
DROP TABLE IF EXISTS "UserManagement"."tenant_memberships";
DROP TABLE IF EXISTS "UserManagement"."tenant_subscriptions";
DROP TABLE IF EXISTS "UserManagement"."tenants";
DROP TABLE IF EXISTS "UserManagement"."role_permissions";
DROP TABLE IF EXISTS "UserManagement"."permissions";
DROP TABLE IF EXISTS "UserManagement"."roles";
DROP TABLE IF EXISTS "UserManagement"."user_mfa_settings";
DROP TABLE IF EXISTS "UserManagement"."refresh_tokens";
DROP TABLE IF EXISTS "UserManagement"."magic_link_tokens";
DROP TABLE IF EXISTS "UserManagement"."mfa_challenges";
DROP TABLE IF EXISTS "UserManagement"."m2m_clients";
DROP TABLE IF EXISTS "UserManagement"."products";
DROP TABLE IF EXISTS "UserManagement"."users";

-- Re-enable foreign key checks
SET session_replication_role = 'origin';

-- Drop the schema (optional – comment out if you want to keep schema)
DROP SCHEMA IF EXISTS "UserManagement" CASCADE;

-- Note: Flyway will automatically remove the migration record from history table