-- PostgreSQL Database Initialization Script
-- Note: Make sure you are connected to the 'sql-travel' database before running this script

-- Create schemas
CREATE SCHEMA IF NOT EXISTS gstusr;
CREATE SCHEMA IF NOT EXISTS usr;
CREATE SCHEMA IF NOT EXISTS supusr;

-- Drop old versions first, as PostgreSQL cannot change the columns of a view
-- or the OUT parameters of a function with CREATE OR REPLACE
DROP VIEW IF EXISTS gstusr."vwInfoDb";
DROP VIEW IF EXISTS gstusr."vwInfoCountries";
DROP FUNCTION IF EXISTS supusr."spDeleteAll"(BOOLEAN);

-- Create views
-- Overview of the database content: number of travelers, cities, attractions and comments
CREATE OR REPLACE VIEW gstusr."vwInfoDb" AS
    SELECT (SELECT COUNT(*) FROM supusr."Travelers" WHERE "Seeded" = true) as "NrSeededTravelers",
        (SELECT COUNT(*) FROM supusr."Travelers" WHERE "Seeded" = false) as "NrUnseededTravelers",
        (SELECT COUNT(*) FROM supusr."Cities" WHERE "Seeded" = true) as "NrSeededCities",
        (SELECT COUNT(*) FROM supusr."Cities" WHERE "Seeded" = false) as "NrUnseededCities",
        (SELECT COUNT(*) FROM supusr."Attractions" WHERE "Seeded" = true) as "NrSeededAttractions",
        (SELECT COUNT(*) FROM supusr."Attractions" WHERE "Seeded" = false) as "NrUnseededAttractions",
        (SELECT COUNT(*) FROM supusr."Attractions" a
            WHERE NOT EXISTS (SELECT 1 FROM supusr."Comments" c WHERE c."AttractionId" = a."AttractionId")) as "NrAttractionsWithoutComments",
        (SELECT COUNT(*) FROM supusr."Comments" WHERE "Seeded" = true) as "NrSeededComments",
        (SELECT COUNT(*) FROM supusr."Comments" WHERE "Seeded" = false) as "NrUnseededComments";

-- Number of cities and attractions per country
CREATE OR REPLACE VIEW gstusr."vwInfoCountries" AS
    SELECT c."Country" as "Country",
        COUNT(DISTINCT c."CityId") as "NrCities",
        COUNT(a."AttractionId") as "NrAttractions"
    FROM supusr."Cities" c
    LEFT JOIN supusr."Attractions" a ON a."CityId" = c."CityId"
    GROUP BY c."Country";

-- Create the DeleteAll function (PostgreSQL uses functions instead of procedures for this pattern)
CREATE OR REPLACE FUNCTION supusr."spDeleteAll"(
    seededParam BOOLEAN DEFAULT true,
    OUT nrTravelersAffected INTEGER,
    OUT nrCitiesAffected INTEGER,
    OUT nrAttractionsAffected INTEGER,
    OUT nrCommentsAffected INTEGER
)
RETURNS RECORD
LANGUAGE plpgsql
AS $$
BEGIN
    SELECT COUNT(*) INTO nrTravelersAffected FROM supusr."Travelers" WHERE "Seeded" = seededParam;
    SELECT COUNT(*) INTO nrCitiesAffected FROM supusr."Cities" WHERE "Seeded" = seededParam;
    SELECT COUNT(*) INTO nrAttractionsAffected FROM supusr."Attractions" WHERE "Seeded" = seededParam;
    SELECT COUNT(*) INTO nrCommentsAffected FROM supusr."Comments" WHERE "Seeded" = seededParam;

    -- Delete in an order that respects the foreign keys:
    -- Comments -> Attractions -> Cities (Restrict), and Travelers
    DELETE FROM supusr."Comments" WHERE "Seeded" = seededParam;
    DELETE FROM supusr."Attractions" WHERE "Seeded" = seededParam;
    DELETE FROM supusr."Cities" WHERE "Seeded" = seededParam;
    DELETE FROM supusr."Travelers" WHERE "Seeded" = seededParam;

    -- Test to throw an error (uncomment if needed)
    -- RAISE EXCEPTION 'Error occurred in supusr.spDeleteAll';
END;
$$;

-- Create Login function
CREATE OR REPLACE FUNCTION gstusr."spLogin"(
    usernameoremail VARCHAR(100),
    userpassword VARCHAR(200),
    OUT userid UUID,
    OUT username VARCHAR(100),
    OUT userrole VARCHAR(100)
)
RETURNS RECORD
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
BEGIN
    userid := NULL;
    username := NULL;
    userrole := NULL;

    SELECT u."UserId", u."UserName", u."UserRole" 
    INTO userid, username, userrole
    FROM dbo."Users" u
    WHERE (u."UserName" = usernameoremail OR (u."Email" IS NOT NULL AND u."Email" = usernameoremail))
      AND u."Password" = userpassword
    LIMIT 1;

    IF userid IS NULL THEN
        RAISE EXCEPTION 'Login error: wrong user or password';
    END IF;
END;
$$;


-- User and role management in PostgreSQL
-- Create roles (PostgreSQL roles are both users and groups)
DO $BODY$
BEGIN
    -- Create login roles
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'gstusr') THEN
        CREATE ROLE gstusr WITH LOGIN PASSWORD 'pa$Word1';
    END IF;
    
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'usr') THEN
        CREATE ROLE usr WITH LOGIN PASSWORD 'pa$Word1';
    END IF;
    
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'supusr') THEN
        CREATE ROLE supusr WITH LOGIN PASSWORD 'pa$Word1';
    END IF;
    
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'dbo') THEN
        CREATE ROLE dbo WITH LOGIN PASSWORD 'pa$Word1';
    END IF;
    
    -- Create group roles (note: lowercase names to match PostgreSQL convention)
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'gstusrrole') THEN
        CREATE ROLE gstusrrole;
    END IF;
    
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'usrrole') THEN
        CREATE ROLE usrrole;
    END IF;
    
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'supusrrole') THEN
        CREATE ROLE supusrrole;
    END IF;
    
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'dborole') THEN
        CREATE ROLE dborole;
    END IF;
END
$BODY$;

-- Grant database connection privileges
GRANT CONNECT ON DATABASE "sql-travel" TO gstusr;
GRANT CONNECT ON DATABASE "sql-travel" TO usr;
GRANT CONNECT ON DATABASE "sql-travel" TO supusr;
GRANT CONNECT ON DATABASE "sql-travel" TO dbo;

-- Grant schema usage privileges
GRANT USAGE ON SCHEMA gstusr TO gstusrrole;
GRANT USAGE ON SCHEMA supusr TO gstusrrole;
GRANT USAGE ON SCHEMA public TO gstusrrole;

-- Grant role privileges for gstusrrole
GRANT SELECT ON gstusr."vwInfoDb" TO gstusrrole;
GRANT SELECT ON gstusr."vwInfoCountries" TO gstusrrole;
GRANT EXECUTE ON FUNCTION gstusr."spLogin"(VARCHAR, VARCHAR) TO gstusrrole;

-- Grant role privileges for usrrole
GRANT USAGE ON SCHEMA supusr TO usrrole;
GRANT SELECT, UPDATE, INSERT ON ALL TABLES IN SCHEMA supusr TO usrrole;

-- Grant role privileges for supusrrole (inherit from usrrole)
GRANT DELETE ON ALL TABLES IN SCHEMA supusr TO supusrrole;
GRANT EXECUTE ON FUNCTION supusr."spDeleteAll"(BOOLEAN) TO supusrrole;

-- Grant role privileges for dborole (full privileges)
GRANT ALL PRIVILEGES ON DATABASE "sql-travel" TO dborole;
-- Grant superuser-like privileges (alternative: ALTER ROLE dborole SUPERUSER;)
GRANT CREATE ON DATABASE "sql-travel" TO dborole;
GRANT ALL ON ALL TABLES IN SCHEMA gstusr, usr, supusr, dbo, public TO dborole;
GRANT ALL ON ALL SEQUENCES IN SCHEMA gstusr, usr, supusr, dbo, public TO dborole;
GRANT ALL ON ALL FUNCTIONS IN SCHEMA gstusr, usr, supusr, dbo, public TO dborole;
GRANT USAGE, CREATE ON SCHEMA gstusr, usr, supusr, dbo, public TO dborole;

-- Assign users to roles
GRANT gstusrrole TO gstusr;

GRANT gstusrrole TO usr;
GRANT usrrole TO usr;

GRANT gstusrrole TO supusr;
GRANT usrrole TO supusr;
GRANT supusrrole TO supusr;

GRANT dborole TO dbo;

