SELECT m.MA_ID, pn.PN_NAme, pv.PV_Value, s.ST_Dist_Type as distribution, s.ST_Mean as Mean, s.ST_Deviation as Deviation 
FROM ParameterNames as pn
LEFT JOIN Stochast as s ON s.PN_ID = pn.PN_ID
LEFT JOIN ParameterValues as pv ON pn.PN_ID = pv.PN_ID
JOIN Materials as m ON m.MA_ID = pv.MA_ID OR m.MA_ID = s.MA_ID
ORDER BY m.MA_ID