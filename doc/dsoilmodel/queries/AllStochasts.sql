SELECT m.MA_ID as Material, pn.PN_Name, s.ST_DistType as distribution, s.ST_Mean as Mean, s.ST_Deviation as Deviation
FROM ParameterNames as pn
JOIN Stochast as s ON s.PN_ID = pn.PN_ID