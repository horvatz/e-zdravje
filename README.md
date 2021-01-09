# eZdravje :hospital:
Aplikacija eZdravje omogoča lažje poslovanje zdravstvenim ustanovam in jih popolnoma digitalizira. Zdravstvenim delavcem omogoča vpisovanje pacientov, izdajanje receptov ter napotnic. Pacientom pa omogoča pregledovanje svoje kartoteke preko spletne aplikacije.

## Avtorja :construction_worker:
**Žan Horvat (63190120)** - *zadolžen za spletno aplikacijo ter podatkovno bazo* <br>
**Žan Mencigar (63190187)** - *zadolžen za Android odjemalec in API vmesnik* <br>

## Kako deluje? :information_source:
<b>Zdravstvena ustanova</b> je administrator, ki dodaja zdravstvene delavce in kategorije zdravnikov (specializacije). Vsak uporabnik (zdravnik ali pacient) rabi <b>aktivacijsko kodo</b> za registracijo na portal eZdravje. Zdravnikom kodo izda administrator, pacientom pa zdravnik.

Nato se uporabniki registrirajo s svojim e-naslovom in aktivacijsko kodo.

## Kaj omogoča? :computer:
Aplikacija ima **3 vrste uporabnikov**:
* Administrator
* Zdravnik
* Pacient

Spodaj so opisane naloge, ki jih lahko izvaja posamezen uporabnik.

### Administrator
Administrator vidi **vse paciente** v sistemu eZdravje.

Administrator lahko:
* Izdaja napotnice in recepte **vsem pacientom**
* Dodaja in spreminja **zdravnike** ter **specializacije zdravnikov** 
* Dodaja **paciente** in njihove **zdravnike** vsem v sistemu

Administratorji so ročno določeni v podatkovni bazi.

### Zdravnik
Status zdravnik dobijo vsi uporabniki, ki so dodani kot zdravniki v sistem eZdravje.

Zdravnik lahko vidi **le svoje paciente**, torej samo tiste katerim je izbran osebni zdravnik.

**Napotnice** lahko izdaja le **svojim** pacientom v sistemu, medtem ko **recepte** lahko izdaja vsem v sistemu eZdravje (če je npr. dežuren zdravnik in pride pacient, ki ni njegov).

### Pacient
Vsak pacient v sistemu dobi uporabniški način "pacient". V tem načinu lahko pregleduje svoje napotnice ter recepte.

## Mobilna aplikacija

## Podatkovna baza
