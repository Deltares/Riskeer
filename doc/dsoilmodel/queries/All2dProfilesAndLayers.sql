SELECT p.SP2D_Name, l.GeometrySurface, mat.MA_Name
FROM MechanismPointLocation as m 
JOIN MechanismPointLocation as mpl ON p.SP2D_ID = mpl.SP2D_ID 
JOIN SoilProfile2D as p ON m.SP2D_ID = p.SP2D_ID 
JOIN SoilLayer2D as l ON l.SP2D_ID = p.SP2D_ID 
JOIN Materials as mat ON mat.MA_ID = l.MA_ID
WHERE m.ME_ID = 4
ORDER BY p.SP2D_ID, l.SP2D_ID