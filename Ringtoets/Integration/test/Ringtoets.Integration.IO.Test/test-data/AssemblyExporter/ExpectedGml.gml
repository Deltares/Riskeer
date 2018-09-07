<?xml version="1.0" encoding="utf-8"?>
<asm:Assemblage xmlns:gml="http://www.opengis.net/gml/3.2" gml:id="Assemblage.0" xmlns:asm="http://localhost/standaarden/assemblage">
  <gml:boundedBy>
    <gml:Envelope>
      <gml:lowerCorner>1 1</gml:lowerCorner>
      <gml:upperCorner>2 2</gml:upperCorner>
    </gml:Envelope>
  </gml:boundedBy>
  <asm:featureMember>
    <asm:Waterkeringstelsel gml:id="Wks.assessmentSectionId">
      <asm:naam>assessmentSectionName</asm:naam>
      <asm:geometrie2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>1 1 2 2</gml:posList>
        </gml:LineString>
      </asm:geometrie2D>
      <asm:lengte uom="m">1.4142135623730952</asm:lengte>
      <asm:typeWaterkeringstelsel>Dijktraject</asm:typeWaterkeringstelsel>
    </asm:Waterkeringstelsel>
    <asm:Beoordelingsproces BeoordelingsprocesID="Bp.0" WaterkeringstelselIDRef="Wks.assessmentSectionId">
      <asm:beginJaarBeoordelingsronde>2017</asm:beginJaarBeoordelingsronde>
      <asm:eindJaarBeoordelingsronde>2023</asm:eindJaarBeoordelingsronde>
    </asm:Beoordelingsproces>
    <asm:Veiligheidsoordeel VeiligheidsoordeelID="Vo.0" BeoordelingsprocesIDRef="Bp.0">
      <asm:veiligheidsoordeel>
        <asm:assemblagemethode>WBI-2C-1</asm:assemblagemethode>
        <asm:categorie>A</asm:categorie>
        <asm:status>VOLLDG</asm:status>
      </asm:veiligheidsoordeel>
      <asm:toetsoordeelMetKansschatting>
        <asm:assemblagemethode>WBI-2B-1</asm:assemblagemethode>
        <asm:categorieTraject>III-traject</asm:categorieTraject>
        <asm:faalkans>0.75</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeelMetKansschatting>
      <asm:toetsoordeelZonderKansschatting>
        <asm:assemblagemethode>WBI-2A-1</asm:assemblagemethode>
        <asm:categorieTraject>III-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeelZonderKansschatting>
    </asm:Veiligheidsoordeel>
    <asm:Toetsspoor ToetsspoorID="Ts.0" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeToetsspoor>STPH</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEDSPROBEX</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:categorieTraject>III-traject</asm:categorieTraject>
        <asm:faalkans>1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.1" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeToetsspoor>STBI</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEDSPROBEX</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:categorieTraject>III-traject</asm:categorieTraject>
        <asm:faalkans>1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.2" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeToetsspoor>GEKB</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEDPROB</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:categorieTraject>III-traject</asm:categorieTraject>
        <asm:faalkans>1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.3" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeToetsspoor>HTKW</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEDPROB</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:categorieTraject>III-traject</asm:categorieTraject>
        <asm:faalkans>1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.4" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeToetsspoor>BSKW</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEDPROB</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:categorieTraject>III-traject</asm:categorieTraject>
        <asm:faalkans>1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.5" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeToetsspoor>STKWp</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEDPROB</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1B-1</asm:assemblagemethode>
        <asm:categorieTraject>III-traject</asm:categorieTraject>
        <asm:faalkans>1</asm:faalkans>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.6" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeToetsspoor>ZST</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEDSPROB</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.7" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeToetsspoor>AGK</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEDSPROB</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.8" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeToetsspoor>GEBU</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEDSPROB</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.9" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeToetsspoor>DA</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEDSPROB</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>NVT</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.10" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeToetsspoor>STBU</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEEN</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.11" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeToetsspoor>STMI</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEEN</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.12" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeToetsspoor>GABU</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEEN</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.13" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeToetsspoor>GABI</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEEN</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.14" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeToetsspoor>PKW</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEEN</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.15" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeToetsspoor>AWO</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEEN</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.16" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeToetsspoor>STKWl</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEEN</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toetsspoor ToetsspoorID="Ts.17" VeiligheidsoordeelIDRef="Vo.0">
      <asm:typeToetsspoor>INN</asm:typeToetsspoor>
      <asm:toetsspoorGroep>GEEN</asm:toetsspoorGroep>
      <asm:typeFaalmechanisme>DIRECT</asm:typeFaalmechanisme>
      <asm:toetsoordeel>
        <asm:assemblagemethode>WBI-1A-1</asm:assemblagemethode>
        <asm:categorieTraject>II-traject</asm:categorieTraject>
        <asm:status>VOLLDG</asm:status>
      </asm:toetsoordeel>
    </asm:Toetsspoor>
    <asm:Toets ToetsID="T.0" ToetsspoorIDRef="Ts.0" WaterkeringsectieIDRef="Wks.0">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-5</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-5</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.1" ToetsspoorIDRef="Ts.0" WaterkeringsectieIDRef="Wks.1">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-5</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-5</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.2" ToetsspoorIDRef="Ts.1" WaterkeringsectieIDRef="Wks.2">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-5</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-5</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.3" ToetsspoorIDRef="Ts.1" WaterkeringsectieIDRef="Wks.3">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-5</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-5</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.4" ToetsspoorIDRef="Ts.2" WaterkeringsectieIDRef="Wks.4">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-3</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-3</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-3</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.5" ToetsspoorIDRef="Ts.2" WaterkeringsectieIDRef="Wks.5">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-3</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-3</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-3</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.6" ToetsspoorIDRef="Ts.3" WaterkeringsectieIDRef="Wks.6">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-3</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-3</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.7" ToetsspoorIDRef="Ts.3" WaterkeringsectieIDRef="Wks.7">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-3</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-3</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.8" ToetsspoorIDRef="Ts.4" WaterkeringsectieIDRef="Wks.8">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-3</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-3</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.9" ToetsspoorIDRef="Ts.4" WaterkeringsectieIDRef="Wks.9">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-3</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-3</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.10" ToetsspoorIDRef="Ts.5" WaterkeringsectieIDRef="Wks.10">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-3</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-3</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-3</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.11" ToetsspoorIDRef="Ts.5" WaterkeringsectieIDRef="Wks.11">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-3</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-3</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
        <asm:faalkans>0.25</asm:faalkans>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-3</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:faalkans>1</asm:faalkans>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.12" ToetsspoorIDRef="Ts.6" WaterkeringsectieIDRef="Wks.12">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-3</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-6</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-4</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.13" ToetsspoorIDRef="Ts.6" WaterkeringsectieIDRef="Wks.13">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-3</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-6</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-4</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.14" ToetsspoorIDRef="Ts.7" WaterkeringsectieIDRef="Wks.14">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-6</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-4</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.15" ToetsspoorIDRef="Ts.7" WaterkeringsectieIDRef="Wks.15">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-6</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-4</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.16" ToetsspoorIDRef="Ts.8" WaterkeringsectieIDRef="Wks.16">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-6</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-4</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.17" ToetsspoorIDRef="Ts.8" WaterkeringsectieIDRef="Wks.17">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-6</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-4</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.18" ToetsspoorIDRef="Ts.10" WaterkeringsectieIDRef="Wks.18">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-3</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-7</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.19" ToetsspoorIDRef="Ts.10" WaterkeringsectieIDRef="Wks.19">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-3</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>IV-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-7</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>VI-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.20" ToetsspoorIDRef="Ts.11" WaterkeringsectieIDRef="Wks.20">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-1</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.21" ToetsspoorIDRef="Ts.11" WaterkeringsectieIDRef="Wks.21">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-1</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.22" ToetsspoorIDRef="Ts.12" WaterkeringsectieIDRef="Wks.22">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-1</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.23" ToetsspoorIDRef="Ts.12" WaterkeringsectieIDRef="Wks.23">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-1</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.24" ToetsspoorIDRef="Ts.13" WaterkeringsectieIDRef="Wks.24">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-1</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.25" ToetsspoorIDRef="Ts.13" WaterkeringsectieIDRef="Wks.25">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-1</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.26" ToetsspoorIDRef="Ts.14" WaterkeringsectieIDRef="Wks.26">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-1</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.27" ToetsspoorIDRef="Ts.14" WaterkeringsectieIDRef="Wks.27">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0G-1</asm:assemblagemethode>
        <asm:toets>GEDTETS</asm:toets>
        <asm:categorieVak>II-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.28" ToetsspoorIDRef="Ts.15" WaterkeringsectieIDRef="Wks.28">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.29" ToetsspoorIDRef="Ts.15" WaterkeringsectieIDRef="Wks.29">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.30" ToetsspoorIDRef="Ts.16" WaterkeringsectieIDRef="Wks.30">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.31" ToetsspoorIDRef="Ts.16" WaterkeringsectieIDRef="Wks.31">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.32" ToetsspoorIDRef="Ts.17" WaterkeringsectieIDRef="Wks.32">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:Toets ToetsID="T.33" ToetsspoorIDRef="Ts.17" WaterkeringsectieIDRef="Wks.33">
      <asm:eindtoetsoordeel>
        <asm:assemblagemethode>WBI-0A-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:eindtoetsoordeel>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0E-1</asm:assemblagemethode>
        <asm:toets>EENVDGETS</asm:toets>
        <asm:categorieVak>VII-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
      <asm:toetsoordeelVak>
        <asm:assemblagemethode>WBI-0T-1</asm:assemblagemethode>
        <asm:toets>TOETSOPMT</asm:toets>
        <asm:categorieVak>I-vak</asm:categorieVak>
      </asm:toetsoordeelVak>
    </asm:Toets>
    <asm:GecombineerdToetsoordeel GecombineerdToetsoordeelID="Gto.0" VeiligheidsoordeelIDRef="Vo.0" WaterkeringsectieIDRef="Wks.34">
      <asm:toetsoordeelGecombineerd>
        <asm:assemblagemethode>WBI-3C-1</asm:assemblagemethode>
        <asm:toets>GECBNTR</asm:toets>
        <asm:categorieVak>III-vak</asm:categorieVak>
      </asm:toetsoordeelGecombineerd>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>STPH</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>GEKB</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>STBI</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>STBU</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>STMI</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>ZST</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>AGK</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>AWO</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>GEBU</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>GABU</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>GABI</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>HTKW</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>BSKW</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>PKW</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>STKWp</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>STKWl</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>DA</asm:typeToetsspoor>
        <asm:categorieVak>NVT</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
      <asm:eindtoetsoordeelToetsspoor>
        <asm:assemblagemethode>WBI-3B-1</asm:assemblagemethode>
        <asm:typeToetsspoor>INN</asm:typeToetsspoor>
        <asm:categorieVak>VI-vak</asm:categorieVak>
        <asm:status>VOLLDG</asm:status>
      </asm:eindtoetsoordeelToetsspoor>
    </asm:GecombineerdToetsoordeel>
    <asm:Vakindeling VakindelingID="Vi.0" ToetsspoorIDRef="Ts.0" />
    <asm:Vakindeling VakindelingID="Vi.1" ToetsspoorIDRef="Ts.1" />
    <asm:Vakindeling VakindelingID="Vi.2" ToetsspoorIDRef="Ts.2" />
    <asm:Vakindeling VakindelingID="Vi.3" ToetsspoorIDRef="Ts.3" />
    <asm:Vakindeling VakindelingID="Vi.4" ToetsspoorIDRef="Ts.4" />
    <asm:Vakindeling VakindelingID="Vi.5" ToetsspoorIDRef="Ts.5" />
    <asm:Vakindeling VakindelingID="Vi.6" ToetsspoorIDRef="Ts.6" />
    <asm:Vakindeling VakindelingID="Vi.7" ToetsspoorIDRef="Ts.7" />
    <asm:Vakindeling VakindelingID="Vi.8" ToetsspoorIDRef="Ts.8" />
    <asm:Vakindeling VakindelingID="Vi.9" ToetsspoorIDRef="Ts.9" />
    <asm:Vakindeling VakindelingID="Vi.10" ToetsspoorIDRef="Ts.10" />
    <asm:Vakindeling VakindelingID="Vi.11" ToetsspoorIDRef="Ts.11" />
    <asm:Vakindeling VakindelingID="Vi.12" ToetsspoorIDRef="Ts.12" />
    <asm:Vakindeling VakindelingID="Vi.13" ToetsspoorIDRef="Ts.13" />
    <asm:Vakindeling VakindelingID="Vi.14" ToetsspoorIDRef="Ts.14" />
    <asm:Vakindeling VakindelingID="Vi.15" ToetsspoorIDRef="Ts.15" />
    <asm:Vakindeling VakindelingID="Vi.16" ToetsspoorIDRef="Ts.16" />
    <asm:Vakindeling VakindelingID="Vi.17" ToetsspoorIDRef="Ts.17" />
    <asm:Vakindeling VakindelingID="Vi.18" VeiligheidsoordeelIDRef="Vo.0" />
    <asm:Waterkeringsectie gml:id="Wks.0" VakindelingIDRef="Vi.0">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.1" VakindelingIDRef="Vi.0">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.2" VakindelingIDRef="Vi.1">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.3" VakindelingIDRef="Vi.1">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.4" VakindelingIDRef="Vi.2">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.5" VakindelingIDRef="Vi.2">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.6" VakindelingIDRef="Vi.3">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.7" VakindelingIDRef="Vi.3">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.8" VakindelingIDRef="Vi.4">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.9" VakindelingIDRef="Vi.4">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.10" VakindelingIDRef="Vi.5">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.11" VakindelingIDRef="Vi.5">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.12" VakindelingIDRef="Vi.6">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.13" VakindelingIDRef="Vi.6">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.14" VakindelingIDRef="Vi.7">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.15" VakindelingIDRef="Vi.7">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.16" VakindelingIDRef="Vi.8">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.17" VakindelingIDRef="Vi.8">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.18" VakindelingIDRef="Vi.10">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.19" VakindelingIDRef="Vi.10">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.20" VakindelingIDRef="Vi.11">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.21" VakindelingIDRef="Vi.11">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.22" VakindelingIDRef="Vi.12">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.23" VakindelingIDRef="Vi.12">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.24" VakindelingIDRef="Vi.13">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.25" VakindelingIDRef="Vi.13">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.26" VakindelingIDRef="Vi.14">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.27" VakindelingIDRef="Vi.14">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.28" VakindelingIDRef="Vi.15">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.29" VakindelingIDRef="Vi.15">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.30" VakindelingIDRef="Vi.16">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.31" VakindelingIDRef="Vi.16">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.32" VakindelingIDRef="Vi.17">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">11.313708498984761</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>-1 -1 7 7</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.33" VakindelingIDRef="Vi.17">
      <asm:afstandBegin uom="m">11.313708498984761</asm:afstandBegin>
      <asm:afstandEinde uom="m">22.627416997969522</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>7 7 15 15</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">11.313708498984761</asm:lengte>
      <asm:typeWaterkeringsectie>TOETSSSTE</asm:typeWaterkeringsectie>
    </asm:Waterkeringsectie>
    <asm:Waterkeringsectie gml:id="Wks.34" VakindelingIDRef="Vi.18">
      <asm:afstandBegin uom="m">0</asm:afstandBegin>
      <asm:afstandEinde uom="m">1</asm:afstandEinde>
      <asm:geometrieLijn2D>
        <gml:LineString srsName="EPSG:28992">
          <gml:posList>1 1 1.70710678118655 1.70710678118655</gml:posList>
        </gml:LineString>
      </asm:geometrieLijn2D>
      <asm:lengte uom="m">0.99999999999999989</asm:lengte>
      <asm:typeWaterkeringsectie>GECBNETSSTE</asm:typeWaterkeringsectie>
      <asm:assemblagemethode>WBI-3A-1</asm:assemblagemethode>
    </asm:Waterkeringsectie>
  </asm:featureMember>
</asm:Assemblage>