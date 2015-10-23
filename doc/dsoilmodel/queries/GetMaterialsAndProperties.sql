SELECT m.MA_Name as material, pn.PN_Name as parameter, pv.PV_Value as value
FROM ParameterNames as pn
JOIN ParameterValues as pv ON pn.PN_ID = pv.PN_ID
JOIN Materials as m ON m.MA_ID = pv.MA_ID