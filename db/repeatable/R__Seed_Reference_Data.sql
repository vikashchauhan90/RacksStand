-- =====================================================
-- Flyway Repeatable Migration: R__Seed_Reference_Data
-- Description: Seed reference data for UserManagement
-- Author: RacksStands Team
-- This script runs every time its checksum changes
-- =====================================================

-- Seed system roles (tenant-level roles)
INSERT INTO "UserManagement".roles (id, name, description, tenant_id, is_system, concurrency_stamp, created_at)
VALUES 
    ('11111111-1111-1111-1111-111111111111', 'Admin', 'Full system administrator', 'system', true, '1', NOW()),
    ('22222222-2222-2222-2222-222222222222', 'User', 'Standard user', 'system', true, '1', NOW()),
    ('33333333-3333-3333-3333-333333333333', 'Viewer', 'Read-only access', 'system', true, '1', NOW())
ON CONFLICT (id) DO UPDATE SET
    name = EXCLUDED.name,
    description = EXCLUDED.description,
    updated_at = NOW();

-- Seed system permissions
INSERT INTO "UserManagement".permissions (id, name, description, "group", tenant_id, is_system, created_at)
VALUES 
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'users:read', 'View user details', 'Users', 'system', true, NOW()),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'users:write', 'Create and update users', 'Users', 'system', true, NOW()),
    ('cccccccc-cccc-cccc-cccc-cccccccccccc', 'users:delete', 'Delete users', 'Users', 'system', true, NOW()),
    ('dddddddd-dddd-dddd-dddd-dddddddddddd', 'roles:read', 'View roles and permissions', 'Roles', 'system', true, NOW()),
    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'roles:write', 'Manage roles and permissions', 'Roles', 'system', true, NOW()),
    ('ffffffff-ffff-ffff-ffff-ffffffffffff', 'tenants:read', 'View tenant information', 'Tenants', 'system', true, NOW()),
    ('11111111-2222-3333-4444-555555555555', 'tenants:manage', 'Manage tenant settings', 'Tenants', 'system', true, NOW())
ON CONFLICT (id) DO UPDATE SET
    name = EXCLUDED.name,
    description = EXCLUDED.description,
    updated_at = NOW();

-- Assign permissions to Admin role (many-to-many)
INSERT INTO "UserManagement".role_permissions (role_id, permission_id)
SELECT 
    r.id,
    p.id
FROM "UserManagement".roles r
CROSS JOIN "UserManagement".permissions p
WHERE r.name = 'Admin'
ON CONFLICT (role_id, permission_id) DO NOTHING;

-- Assign basic permissions to User role
INSERT INTO "UserManagement".role_permissions (role_id, permission_id)
SELECT 
    r.id,
    p.id
FROM "UserManagement".roles r
CROSS JOIN "UserManagement".permissions p
WHERE r.name = 'User' 
  AND p.name IN ('users:read', 'users:write')
ON CONFLICT (role_id, permission_id) DO NOTHING;

-- Assign read-only permissions to Viewer role
INSERT INTO "UserManagement".role_permissions (role_id, permission_id)
SELECT 
    r.id,
    p.id
FROM "UserManagement".roles r
CROSS JOIN "UserManagement".permissions p
WHERE r.name = 'Viewer' 
  AND p.name IN ('users:read', 'roles:read', 'tenants:read')
ON CONFLICT (role_id, permission_id) DO NOTHING;

-- Seed default products (subscription plans)
INSERT INTO "UserManagement".products (id, name, description, created_at)
VALUES 
    ('p001-0001-0001-0001-000000000001', 'Free', 'Free tier with basic features', NOW()),
    ('p001-0001-0001-0001-000000000002', 'Pro', 'Professional tier with advanced features', NOW()),
    ('p001-0001-0001-0001-000000000003', 'Enterprise', 'Full feature set with priority support', NOW())
ON CONFLICT (name) DO UPDATE SET
    description = EXCLUDED.description,
    updated_at = NOW();

-- Seed default tenant (system tenant)
INSERT INTO "UserManagement".tenants (id, name, slug, owner_id, created_at)
VALUES 
    ('tenant-system-0000-0000-000000000001', 'System Tenant', 'system', 'system-owner', NOW())
ON CONFLICT (slug) DO UPDATE SET
    name = EXCLUDED.name,
    updated_at = NOW();

-- Seed default admin user (password should be changed on first login)
-- Note: Password hash is for 'Admin@123' - CHANGE IN PRODUCTION!
INSERT INTO "UserManagement".users (id, name, user_name, email, password_hash, created_at)
VALUES 
    ('admin-user-0000-0000-000000000001', 'System Administrator', 'admin', 'admin@racksstands.com', 
     '$2a$11$K8QxP8LxM8QxP8LxM8QxPeZxM8QxP8LxM8QxP8LxM8QxP8LxM8QxP8L', NOW())
ON CONFLICT (email) DO UPDATE SET
    name = EXCLUDED.name,
    updated_at = NOW();

-- Assign admin user to system tenant
INSERT INTO "UserManagement".tenant_memberships (id, tenant_id, user_id, role_id, assigned_by_user_id, joined_at, created_at)
SELECT 
    gen_random_uuid()::varchar(36),
    t.id,
    u.id,
    r.id,
    u.id,
    NOW(),
    NOW()
FROM "UserManagement".tenants t
CROSS JOIN "UserManagement".users u
CROSS JOIN "UserManagement".roles r
WHERE t.slug = 'system' 
  AND u.email = 'admin@racksstands.com'
  AND r.name = 'Admin'
ON CONFLICT (tenant_id, user_id, deleted_at) WHERE deleted_at IS NULL 
DO NOTHING;