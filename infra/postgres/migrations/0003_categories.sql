CREATE TABLE IF NOT EXISTS categories (
  id UUID PRIMARY KEY,
  name VARCHAR(100) NOT NULL,
  description VARCHAR(500) NOT NULL DEFAULT '',
  color VARCHAR(50) NOT NULL DEFAULT '',
  icon VARCHAR(50) NOT NULL DEFAULT '',
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

INSERT INTO categories (id, name, description, color, icon, created_at)
VALUES
  ('20000000-0000-0000-0000-000000000001', 'Work', 'Work and professional tasks', '#3B82F6', 'briefcase', NOW()),
  ('20000000-0000-0000-0000-000000000002', 'Personal', 'Personal errands and daily routine', '#10B981', 'user', NOW())
ON CONFLICT (id) DO NOTHING;
