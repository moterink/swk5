INSERT INTO holidays (start_date, end_date, name, is_school_holiday) VALUES
                                                                         ('2024-12-24', '2025-01-02', 'Christmas Break', TRUE),
                                                                         ('2024-12-25', '2024-12-25', 'Christmas Day', FALSE),
                                                                         ('2024-01-01', '2024-01-01', 'New Years Day', FALSE),
('2024-07-01', '2024-07-31', 'Summer Holidays', TRUE),
('2025-07-01', '2025-07-31', 'Summer Holidays', TRUE),
('2024-04-01', '2024-04-10', 'Easter Break', TRUE),
('2025-04-01', '2025-04-10', 'Easter Break', TRUE);

INSERT INTO stops (name, short_name, latitude, longitude) VALUES
('Wartberg ob der Aist Obervisnitz', 'WART_OB', 48.3432, 14.5162),
('Wartberg ob der Aist Abzw. Scheiben', 'WART_ABZW', 48.3491, 14.5213),
('Wartberg ob der Aist Friensdorf', 'WART_FRIENS', 48.3530, 14.5280),
('Pregarten Bahnhof', 'PREG_BAHN', 48.3687, 14.5402),
('Hagenberg Linzer Kreuzung', 'HAGEN_LINZ', 48.3681, 14.5160),
('Hagenberg Zainze', 'HAGEN_ZAIN', 48.3654, 14.5080),
('Hagenberg Althannstrasse', 'HAGEN_ALT', 48.3633, 14.5055),
('Hagenberg Tumlerstrasse', 'HAGEN_TUM', 48.3610, 14.5030),
('Hagenberg Ortsmitte', 'HAGEN_ORTS', 48.3580, 14.5005),
('Hagenberg Softwarepark', 'HAGEN_SOFT', 48.3540, 14.4980),
('Linz Karlhof', 'LINZ_KARL', 48.3032, 14.2862),
('Linz Ontlstrasse', 'LINZ_ONTL', 48.3061, 14.2901),
('Linz Heilhamer Weg', 'LINZ_HEIL', 48.3090, 14.2955),
('Linz Donauparkstation', 'LINZ_DONAUP', 48.3137, 14.3005),
('Linz Parkbad', 'LINZ_PARK', 48.3167, 14.3033),
('Linz JKU', 'LINZ_JKU', 48.3375, 14.3190),
('Linz Schumpeterstrasse', 'LINZ_SCHUMP', 48.3354, 14.3125),
('Linz Dornach', 'LINZ_DORN', 48.3330, 14.3080),
('Linz Glaserstraße', 'LINZ_GLASER', 48.3305, 14.3055),
('Linz St.Magdalena', 'LINZ_MAGD', 48.3280, 14.3030),
('Linz Ferdinand-Markl-Strasse', 'LINZ_FERDINAND', 48.3260, 14.3005),
('Linz Gründberg', 'LINZ_GRUEND', 48.3235, 14.2980),
('Linz Harbachsiedlung', 'LINZ_HARBACH', 48.3205, 14.2955),
('Linz Harbach', 'LINZ_HARBACH2', 48.3180, 14.2930),
('Linz Linke Brueckenstrasse', 'LINZ_LINK_BRUECK', 48.3160, 14.2915),
('Linz Peuerbachstrasse', 'LINZ_PEUE', 48.3145, 14.2900),
('Linz Wildbergstrasse', 'LINZ_WILD', 48.3120, 14.2885),
('Linz Rudolfstrasse', 'LINZ_RUDOLF', 48.3100, 14.2870),
('Linz Hauptplatz', 'LINZ_HAUPTPLATZ', 48.3085, 14.2855),
('Linz Taubenmarkt', 'LINZ_TAUB', 48.3070, 14.2840),
('Linz Mozartkreuzung', 'LINZ_MOZART', 48.3055, 14.2830),
('Linz Buergerstrasse', 'LINZ_BUERG', 48.3040, 14.2815),
('Linz Goethekreuzung', 'LINZ_GOETHE', 48.3025, 14.2800),
('Linz Hauptbahnhof', 'LINZ_HBF', 48.3005, 14.2785);

-- Insert test data for the routes table
INSERT INTO routes (number, valid_from, valid_to, days_of_operation) VALUES
('311', '2024-01-01', '2024-12-31', 'Monday,Tuesday,Wednesday,Thursday,Friday,Weekday'),
('25', '2024-01-01', '2024-12-31', 'Monday,Tuesday,Wednesday,Thursday,Friday,Weekday'),
('2', '2024-01-01', '2024-12-31', 'Monday,Tuesday,Wednesday,Thursday,Friday,Weekday');

-- Insert test data for the route_stops table
INSERT INTO route_stops (route_id, stop_id, sequence_number, scheduled_departure_time) VALUES
-- Route 311
(1, 1, 1, '06:00:00'), -- Wartberg ob der Aist Obervisnitz
(1, 2, 2, '06:05:00'), -- Wartberg ob der Aist Abzw. Scheiben
(1, 3, 3, '06:10:00'), -- Wartberg ob der Aist Friensdorf
(1, 4, 4, '06:15:00'), -- Pregarten Bahnhof
(1, 5, 5, '06:25:00'), -- Hagenberg Linzer Kreuzung
(1, 6, 6, '06:30:00'), -- Hagenberg Zainze
(1, 7, 7, '06:35:00'), -- Hagenberg Althannstrasse
(1, 8, 8, '06:40:00'), -- Hagenberg Tumlerstrasse
(1, 9, 9, '06:45:00'), -- Hagenberg Ortsmitte
(1, 10, 10, '06:50:00'), -- Hagenberg Softwarepark
-- Route 25
(2, 11, 1, '07:00:00'), -- Linz Karlhof
(2, 12, 2, '07:05:00'), -- Linz Ontlstrasse
(2, 13, 3, '07:10:00'), -- Linz Heilhamer Weg
(2, 14, 4, '07:15:00'), -- Linz Donauparkstation
(2, 15, 5, '07:20:00'), -- Linz Parkbad
-- Route 2
(3, 16, 1, '06:30:00'), -- Linz JKU
(3, 17, 2, '06:35:00'), -- Linz Schumpeterstrasse
(3, 18, 3, '06:40:00'), -- Linz Dornach
(3, 19, 4, '06:45:00'), -- Linz Glaserstraße
(3, 20, 5, '06:50:00'), -- Linz St.Magdalena
(3, 21, 6, '06:55:00'), -- Linz Ferdinand-Markl-Strasse
(3, 22, 7, '07:00:00'), -- Linz Gründberg
(3, 23, 8, '07:05:00'), -- Linz Harbachsiedlung
(3, 24, 9, '07:10:00'), -- Linz Harbach
(3, 12, 10, '07:15:00'), -- Linz Ontlstrasse
(3, 25, 11, '07:20:00'), -- Linz Linke Brueckenstrasse
(3, 26, 12, '07:25:00'), -- Linz Peuerbachstrasse
(3, 27, 13, '07:30:00'), -- Linz Wildbergstrasse
(3, 28, 14, '07:35:00'), -- Linz Rudolfstrasse
(3, 29, 15, '07:40:00'), -- Linz Hauptplatz
(3, 30, 16, '07:45:00'), -- Linz Taubenmarkt
(3, 31, 17, '07:50:00'), -- Linz Mozartkreuzung
(3, 32, 18, '07:55:00'), -- Linz Buergerstrasse
(3, 33, 19, '08:00:00'), -- Linz Goethekreuzung
(3, 34, 20, '08:05:00'); -- Linz Hauptbahnhof

-- Insert test data for the checkins table
INSERT INTO checkins (route_id, stop_id, timestamp) VALUES
(1, 1, '2024-12-22 05:55:00'), -- Wartberg ob der Aist Obervisnitz check-in for ROUTE311
(1, 4, '2024-12-22 06:14:00'), -- Pregarten Bahnhof check-in for ROUTE311
(2, 11, '2024-12-22 06:55:00'), -- Linz Karlhof check-in for ROUTE25
(2, 14, '2024-12-22 07:14:00'), -- Linz Donauparkstation check-in for ROUTE25
(3, 16, '2024-12-22 06:25:00'), -- Linz JKU check-in for ROUTE2
(3, 20, '2024-12-22 06:50:00'), -- Linz St.Magdalena check-in for ROUTE2
(3, 34, '2024-12-22 08:05:00'); -- Linz Hauptbahnhof check-in for ROUTE2
