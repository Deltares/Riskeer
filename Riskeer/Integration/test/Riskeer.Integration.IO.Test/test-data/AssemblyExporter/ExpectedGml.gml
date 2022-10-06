<?xml version="1.0" encoding="utf-8"?>
<uboi:Assembleren-collectie xmlns:xlink="http://www.w3.org/1999/xlink" xmlns:gml="http://www.opengis.net/gml/3.2" xmlns:imwap="http://data.aquo.nl/xsd/BOI/imwaproxies/v20220630" gml:id="assemblage.0" xmlns:uboi="http://data.aquo.nl/xsd/BOI/uitwisselmodel/v20220630">
  <uboi:featureMember>
    <imwap:Waterkeringstelsel gml:id="Wks.assessmentSectionId">
      <imwap:geometrie2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>0 0 3 4</gml:posList>
        </gml:LineString>
      </imwap:geometrie2D>
      <imwap:typeWaterkeringstelsel>Dijktraject</imwap:typeWaterkeringstelsel>
      <imwap:naam>assessmentSectionName</imwap:naam>
      <imwap:lengte>5</imwap:lengte>
    </imwap:Waterkeringstelsel>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Beoordelingsproces gml:id="Bp.0">
      <uboi:beginJaarBeoordelingsronde>2023</uboi:beginJaarBeoordelingsronde>
      <uboi:eindJaarBeoordelingsronde>2035</uboi:eindJaarBeoordelingsronde>
      <uboi:beoordeelt xlink:href="Wks.assessmentSectionId" />
    </uboi:Beoordelingsproces>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Veiligheidsoordeel gml:id="Vo.0">
      <uboi:categorie>A+</uboi:categorie>
      <uboi:assemblagemethodeVeiligheidsoordeel>BOI-2B-1</uboi:assemblagemethodeVeiligheidsoordeel>
      <uboi:faalkans>0.14</uboi:faalkans>
      <uboi:assemblagemethodeFaalkans>BOI-2A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:uitkomstVan xlink:href="Bp.0" />
    </uboi:Veiligheidsoordeel>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:GeneriekFaalmechanisme gml:id="Fm.0">
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeFaalkans>BOI-1A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:bepaalt xlink:href="Vo.0" />
      <uboi:generiekFaalmechanisme>STPH</uboi:generiekFaalmechanisme>
    </uboi:GeneriekFaalmechanisme>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.0">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.0" />
      <uboi:geldtVoor xlink:href="Bv.0" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.1">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.0" />
      <uboi:geldtVoor xlink:href="Bv.1" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:GeneriekFaalmechanisme gml:id="Fm.1">
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeFaalkans>BOI-1A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:bepaalt xlink:href="Vo.0" />
      <uboi:generiekFaalmechanisme>GEKB</uboi:generiekFaalmechanisme>
    </uboi:GeneriekFaalmechanisme>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.2">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.1" />
      <uboi:geldtVoor xlink:href="Bv.2" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.3">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.1" />
      <uboi:geldtVoor xlink:href="Bv.3" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:GeneriekFaalmechanisme gml:id="Fm.2">
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeFaalkans>BOI-1A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:bepaalt xlink:href="Vo.0" />
      <uboi:generiekFaalmechanisme>STBI</uboi:generiekFaalmechanisme>
    </uboi:GeneriekFaalmechanisme>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.4">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.2" />
      <uboi:geldtVoor xlink:href="Bv.4" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.5">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.2" />
      <uboi:geldtVoor xlink:href="Bv.5" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:GeneriekFaalmechanisme gml:id="Fm.3">
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeFaalkans>BOI-1A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:bepaalt xlink:href="Vo.0" />
      <uboi:generiekFaalmechanisme>STMI</uboi:generiekFaalmechanisme>
    </uboi:GeneriekFaalmechanisme>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.6">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.3" />
      <uboi:geldtVoor xlink:href="Bv.6" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.7">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.3" />
      <uboi:geldtVoor xlink:href="Bv.7" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:GeneriekFaalmechanisme gml:id="Fm.4">
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeFaalkans>BOI-1A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:bepaalt xlink:href="Vo.0" />
      <uboi:generiekFaalmechanisme>ZST</uboi:generiekFaalmechanisme>
    </uboi:GeneriekFaalmechanisme>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.8">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.4" />
      <uboi:geldtVoor xlink:href="Bv.8" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.9">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.4" />
      <uboi:geldtVoor xlink:href="Bv.9" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:GeneriekFaalmechanisme gml:id="Fm.5">
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeFaalkans>BOI-1A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:bepaalt xlink:href="Vo.0" />
      <uboi:generiekFaalmechanisme>AGK</uboi:generiekFaalmechanisme>
    </uboi:GeneriekFaalmechanisme>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.10">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.5" />
      <uboi:geldtVoor xlink:href="Bv.10" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.11">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.5" />
      <uboi:geldtVoor xlink:href="Bv.11" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:GeneriekFaalmechanisme gml:id="Fm.6">
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeFaalkans>BOI-1A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:bepaalt xlink:href="Vo.0" />
      <uboi:generiekFaalmechanisme>AWO</uboi:generiekFaalmechanisme>
    </uboi:GeneriekFaalmechanisme>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.12">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.6" />
      <uboi:geldtVoor xlink:href="Bv.12" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.13">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.6" />
      <uboi:geldtVoor xlink:href="Bv.13" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:GeneriekFaalmechanisme gml:id="Fm.7">
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeFaalkans>BOI-1A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:bepaalt xlink:href="Vo.0" />
      <uboi:generiekFaalmechanisme>GEBU</uboi:generiekFaalmechanisme>
    </uboi:GeneriekFaalmechanisme>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.14">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.7" />
      <uboi:geldtVoor xlink:href="Bv.14" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.15">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.7" />
      <uboi:geldtVoor xlink:href="Bv.15" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:GeneriekFaalmechanisme gml:id="Fm.8">
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeFaalkans>BOI-1A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:bepaalt xlink:href="Vo.0" />
      <uboi:generiekFaalmechanisme>GABU</uboi:generiekFaalmechanisme>
    </uboi:GeneriekFaalmechanisme>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.16">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.8" />
      <uboi:geldtVoor xlink:href="Bv.16" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.17">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.8" />
      <uboi:geldtVoor xlink:href="Bv.17" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:GeneriekFaalmechanisme gml:id="Fm.9">
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeFaalkans>BOI-1A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:bepaalt xlink:href="Vo.0" />
      <uboi:generiekFaalmechanisme>GABI</uboi:generiekFaalmechanisme>
    </uboi:GeneriekFaalmechanisme>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.18">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.9" />
      <uboi:geldtVoor xlink:href="Bv.18" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.19">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.9" />
      <uboi:geldtVoor xlink:href="Bv.19" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:GeneriekFaalmechanisme gml:id="Fm.10">
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeFaalkans>BOI-1A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:bepaalt xlink:href="Vo.0" />
      <uboi:generiekFaalmechanisme>HTKW</uboi:generiekFaalmechanisme>
    </uboi:GeneriekFaalmechanisme>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.20">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.10" />
      <uboi:geldtVoor xlink:href="Bv.20" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.21">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.10" />
      <uboi:geldtVoor xlink:href="Bv.21" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:GeneriekFaalmechanisme gml:id="Fm.11">
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeFaalkans>BOI-1A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:bepaalt xlink:href="Vo.0" />
      <uboi:generiekFaalmechanisme>BSKW</uboi:generiekFaalmechanisme>
    </uboi:GeneriekFaalmechanisme>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.22">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.11" />
      <uboi:geldtVoor xlink:href="Bv.22" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.23">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.11" />
      <uboi:geldtVoor xlink:href="Bv.23" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:GeneriekFaalmechanisme gml:id="Fm.12">
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeFaalkans>BOI-1A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:bepaalt xlink:href="Vo.0" />
      <uboi:generiekFaalmechanisme>PKW</uboi:generiekFaalmechanisme>
    </uboi:GeneriekFaalmechanisme>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.24">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.12" />
      <uboi:geldtVoor xlink:href="Bv.24" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.25">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.12" />
      <uboi:geldtVoor xlink:href="Bv.25" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:GeneriekFaalmechanisme gml:id="Fm.13">
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeFaalkans>BOI-1A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:bepaalt xlink:href="Vo.0" />
      <uboi:generiekFaalmechanisme>STKWp</uboi:generiekFaalmechanisme>
    </uboi:GeneriekFaalmechanisme>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.26">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.13" />
      <uboi:geldtVoor xlink:href="Bv.26" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.27">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.13" />
      <uboi:geldtVoor xlink:href="Bv.27" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:SpecifiekFaalmechanisme gml:id="Fm.14">
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeFaalkans>BOI-1A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:bepaalt xlink:href="Vo.0" />
      <uboi:specifiekFaalmechanisme>Specific failure mechanism 1</uboi:specifiekFaalmechanisme>
    </uboi:SpecifiekFaalmechanisme>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.28">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.14" />
      <uboi:geldtVoor xlink:href="Bv.28" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.29">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.14" />
      <uboi:geldtVoor xlink:href="Bv.29" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:SpecifiekFaalmechanisme gml:id="Fm.15">
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeFaalkans>BOI-1A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:bepaalt xlink:href="Vo.0" />
      <uboi:specifiekFaalmechanisme>Specific failure mechanism 2</uboi:specifiekFaalmechanisme>
    </uboi:SpecifiekFaalmechanisme>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.30">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.15" />
      <uboi:geldtVoor xlink:href="Bv.30" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Faalanalyse gml:id="Fa.31">
      <uboi:duidingsklasse>+I</uboi:duidingsklasse>
      <uboi:faalkans>0.1</uboi:faalkans>
      <uboi:assemblagemethodeDuidingsklasse>BOI-0B-1</uboi:assemblagemethodeDuidingsklasse>
      <uboi:assemblagemethodeFaalkans>BOI-0A-1</uboi:assemblagemethodeFaalkans>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:analyseert xlink:href="Fm.15" />
      <uboi:geldtVoor xlink:href="Bv.31" />
    </uboi:Faalanalyse>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvakGecombineerd gml:id="Gf.0">
      <uboi:duidingsklasse>+II</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3C-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:specificeert xlink:href="Vo.0" />
      <uboi:geldtVoor xlink:href="Bv.32" />
    </uboi:AnalyseDeelvakGecombineerd>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+II</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.0" />
      <uboi:duidt xlink:href="Gf.0" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+II</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.2" />
      <uboi:duidt xlink:href="Gf.0" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+II</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.4" />
      <uboi:duidt xlink:href="Gf.0" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+II</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.6" />
      <uboi:duidt xlink:href="Gf.0" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+II</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.8" />
      <uboi:duidt xlink:href="Gf.0" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+II</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.10" />
      <uboi:duidt xlink:href="Gf.0" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+II</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.12" />
      <uboi:duidt xlink:href="Gf.0" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+II</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.14" />
      <uboi:duidt xlink:href="Gf.0" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+II</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.16" />
      <uboi:duidt xlink:href="Gf.0" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+II</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.18" />
      <uboi:duidt xlink:href="Gf.0" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+II</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.20" />
      <uboi:duidt xlink:href="Gf.0" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+II</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.22" />
      <uboi:duidt xlink:href="Gf.0" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+II</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.24" />
      <uboi:duidt xlink:href="Gf.0" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+II</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.26" />
      <uboi:duidt xlink:href="Gf.0" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+II</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.28" />
      <uboi:duidt xlink:href="Gf.0" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+II</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.30" />
      <uboi:duidt xlink:href="Gf.0" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvakGecombineerd gml:id="Gf.1">
      <uboi:duidingsklasse>+III</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3C-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:specificeert xlink:href="Vo.0" />
      <uboi:geldtVoor xlink:href="Bv.33" />
    </uboi:AnalyseDeelvakGecombineerd>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+III</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.0" />
      <uboi:duidt xlink:href="Gf.1" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+III</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.2" />
      <uboi:duidt xlink:href="Gf.1" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+III</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.4" />
      <uboi:duidt xlink:href="Gf.1" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+III</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.6" />
      <uboi:duidt xlink:href="Gf.1" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+III</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.8" />
      <uboi:duidt xlink:href="Gf.1" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+III</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.10" />
      <uboi:duidt xlink:href="Gf.1" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+III</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.12" />
      <uboi:duidt xlink:href="Gf.1" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+III</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.14" />
      <uboi:duidt xlink:href="Gf.1" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+III</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.16" />
      <uboi:duidt xlink:href="Gf.1" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+III</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.18" />
      <uboi:duidt xlink:href="Gf.1" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+III</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.20" />
      <uboi:duidt xlink:href="Gf.1" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+III</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.22" />
      <uboi:duidt xlink:href="Gf.1" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+III</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.24" />
      <uboi:duidt xlink:href="Gf.1" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+III</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.26" />
      <uboi:duidt xlink:href="Gf.1" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+III</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.28" />
      <uboi:duidt xlink:href="Gf.1" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:AnalyseDeelvak>
      <uboi:duidingsklasse>+III</uboi:duidingsklasse>
      <uboi:assemblagemethode>BOI-3B-1</uboi:assemblagemethode>
      <uboi:status>VOLLDG</uboi:status>
      <uboi:afgeleidVan xlink:href="Fa.30" />
      <uboi:duidt xlink:href="Gf.1" />
    </uboi:AnalyseDeelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <imwap:Vakindeling gml:id="Vi.0" />
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.0">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>0</imwap:afstandBegin>
      <imwap:afstandEinde>11.313708498984761</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.0" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.1">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>11.313708498984761</imwap:afstandBegin>
      <imwap:afstandEinde>22.627416997969522</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.0" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <imwap:Vakindeling gml:id="Vi.1" />
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.2">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>0</imwap:afstandBegin>
      <imwap:afstandEinde>11.313708498984761</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.1" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.3">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>11.313708498984761</imwap:afstandBegin>
      <imwap:afstandEinde>22.627416997969522</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.1" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <imwap:Vakindeling gml:id="Vi.2" />
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.4">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>0</imwap:afstandBegin>
      <imwap:afstandEinde>11.313708498984761</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.2" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.5">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>11.313708498984761</imwap:afstandBegin>
      <imwap:afstandEinde>22.627416997969522</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.2" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <imwap:Vakindeling gml:id="Vi.3" />
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.6">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>0</imwap:afstandBegin>
      <imwap:afstandEinde>11.313708498984761</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.3" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.7">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>11.313708498984761</imwap:afstandBegin>
      <imwap:afstandEinde>22.627416997969522</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.3" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <imwap:Vakindeling gml:id="Vi.4" />
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.8">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>0</imwap:afstandBegin>
      <imwap:afstandEinde>11.313708498984761</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.4" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.9">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>11.313708498984761</imwap:afstandBegin>
      <imwap:afstandEinde>22.627416997969522</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.4" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <imwap:Vakindeling gml:id="Vi.5" />
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.10">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>0</imwap:afstandBegin>
      <imwap:afstandEinde>11.313708498984761</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.5" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.11">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>11.313708498984761</imwap:afstandBegin>
      <imwap:afstandEinde>22.627416997969522</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.5" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <imwap:Vakindeling gml:id="Vi.6" />
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.12">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>0</imwap:afstandBegin>
      <imwap:afstandEinde>11.313708498984761</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.6" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.13">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>11.313708498984761</imwap:afstandBegin>
      <imwap:afstandEinde>22.627416997969522</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.6" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <imwap:Vakindeling gml:id="Vi.7" />
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.14">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>0</imwap:afstandBegin>
      <imwap:afstandEinde>11.313708498984761</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.7" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.15">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>11.313708498984761</imwap:afstandBegin>
      <imwap:afstandEinde>22.627416997969522</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.7" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <imwap:Vakindeling gml:id="Vi.8" />
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.16">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>0</imwap:afstandBegin>
      <imwap:afstandEinde>11.313708498984761</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.8" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.17">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>11.313708498984761</imwap:afstandBegin>
      <imwap:afstandEinde>22.627416997969522</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.8" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <imwap:Vakindeling gml:id="Vi.9" />
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.18">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>0</imwap:afstandBegin>
      <imwap:afstandEinde>11.313708498984761</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.9" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.19">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>11.313708498984761</imwap:afstandBegin>
      <imwap:afstandEinde>22.627416997969522</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.9" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <imwap:Vakindeling gml:id="Vi.10" />
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.20">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>0</imwap:afstandBegin>
      <imwap:afstandEinde>11.313708498984761</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.10" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.21">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>11.313708498984761</imwap:afstandBegin>
      <imwap:afstandEinde>22.627416997969522</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.10" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <imwap:Vakindeling gml:id="Vi.11" />
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.22">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>0</imwap:afstandBegin>
      <imwap:afstandEinde>11.313708498984761</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.11" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.23">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>11.313708498984761</imwap:afstandBegin>
      <imwap:afstandEinde>22.627416997969522</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.11" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <imwap:Vakindeling gml:id="Vi.12" />
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.24">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>0</imwap:afstandBegin>
      <imwap:afstandEinde>11.313708498984761</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.12" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.25">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>11.313708498984761</imwap:afstandBegin>
      <imwap:afstandEinde>22.627416997969522</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.12" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <imwap:Vakindeling gml:id="Vi.13" />
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.26">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>0</imwap:afstandBegin>
      <imwap:afstandEinde>11.313708498984761</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.13" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.27">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>11.313708498984761</imwap:afstandBegin>
      <imwap:afstandEinde>22.627416997969522</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.13" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <imwap:Vakindeling gml:id="Vi.14" />
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.28">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>0</imwap:afstandBegin>
      <imwap:afstandEinde>11.313708498984761</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.14" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.29">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>11.313708498984761</imwap:afstandBegin>
      <imwap:afstandEinde>22.627416997969522</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.14" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <imwap:Vakindeling gml:id="Vi.15" />
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.30">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>0</imwap:afstandBegin>
      <imwap:afstandEinde>11.313708498984761</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.15" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.31">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>FAALMVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>11.313708498984761</imwap:afstandBegin>
      <imwap:afstandEinde>22.627416997969522</imwap:afstandEinde>
      <imwap:lengte>11.313708498984761</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.15" />
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <imwap:Vakindeling gml:id="Vi.16" />
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.32">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>0 0 1.5 2</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>DEELVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>0</imwap:afstandBegin>
      <imwap:afstandEinde>2.5</imwap:afstandEinde>
      <imwap:lengte>2.5</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.16" />
      <uboi:assemblagemethode>BOI-3A-1</uboi:assemblagemethode>
    </uboi:Deelvak>
  </uboi:featureMember>
  <uboi:featureMember>
    <uboi:Deelvak gml:id="Bv.33">
      <imwap:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>1.5 2 3 4</gml:posList>
        </gml:LineString>
      </imwap:geometrieLijn2D>
      <imwap:typeWaterkeringsectie>DEELVK</imwap:typeWaterkeringsectie>
      <imwap:afstandBegin>2.5</imwap:afstandBegin>
      <imwap:afstandEinde>5</imwap:afstandEinde>
      <imwap:lengte>2.5</imwap:lengte>
      <imwap:onderdeelVan xlink:href="Vi.16" />
      <uboi:assemblagemethode>BOI-3A-1</uboi:assemblagemethode>
    </uboi:Deelvak>
  </uboi:featureMember>
</uboi:Assembleren-collectie>