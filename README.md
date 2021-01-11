# eZdravje :hospital:
Aplikacija eZdravje omogoča lažje poslovanje zdravstvenim ustanovam in jih popolnoma digitalizira. Zdravstvenim delavcem omogoča vpisovanje pacientov, izdajanje receptov ter napotnic. Pacientom pa omogoča pregledovanje svoje kartoteke preko spletne aplikacije.

## Avtorja :construction_worker:
**Žan Horvat (63190120)** - *zadolžen za spletno aplikacijo ter podatkovno bazo* <br>
**Žan Mencigar (63190187)** - *zadolžen za Android odjemalec in API vmesnik* <br>

## Kako deluje? :information_source:
<b>Zdravstvena ustanova</b> je administrator, ki dodaja zdravstvene delavce in kategorije zdravnikov (specializacije). Vsak uporabnik (zdravnik ali pacient) rabi <b>aktivacijsko kodo</b> za registracijo na portal eZdravje. Zdravnikom kodo izda administrator, pacientom pa zdravnik.

Nato se uporabniki registrirajo s svojim e-naslovom in aktivacijsko kodo.


![Domača stran eZdravje](https://github.com/horvatz/eZdravje/blob/master/img/eZdravje_home.PNG)

## Kaj omogoča? :computer:
Aplikacije eZdravje omogoča pregled nad **pacienti**, pregled in izdajanje **napotnic** ter **receptov** in administracijo nad **zdravniki**.

Delovanje aplikacije je odvisno od vrste uporabnika.

Aplikacija ima **3 vrste uporabnikov**:
* Administrator
* Zdravnik
* Pacient

Spodaj so opisane naloge, ki jih lahko izvaja posamezen uporabnik.

### Administrator :lock:
Administrator vidi **vse paciente** v sistemu eZdravje.

Administrator lahko:
* Izdaja napotnice in recepte **vsem pacientom**
* Dodaja in spreminja **zdravnike** ter **specializacije zdravnikov** 
* Dodaja **paciente** in njihove **zdravnike** vsem v sistemu

Administratorji so ročno določeni v podatkovni bazi.

Spodaj je prikazan **dodajanje pacienta** v spletni aplikaciji. Vsak pacient ima tudi svojega izbranega osebnega zdravnika:

![Dodajanje pacienta v aplikaciji eZdravje](https://github.com/horvatz/eZdravje/blob/master/img/eZdravje_add_patient.PNG)

### Zdravnik :syringe:
Status zdravnik dobijo vsi uporabniki, ki so dodani kot zdravniki v sistem eZdravje.

Zdravnik lahko vidi **le svoje paciente**, torej samo tiste katerim je izbran osebni zdravnik.

**Napotnice** lahko izdaja le **svojim** pacientom v sistemu, medtem ko **recepte** lahko izdaja vsem v sistemu eZdravje (če je npr. dežuren zdravnik in pride pacient, ki ni njegov).


![Recepti v aplikaciji eZdravje](https://github.com/horvatz/eZdravje/blob/master/img/eZdravje_prescriptions.PNG)

### Pacient :man:
Vsak pacient v sistemu dobi uporabniški način "pacient". V tem načinu lahko pregleduje svoje napotnice ter recepte.

Vsak uporabnik (tudi neprijavljen) lahko vidi zdravnike in specializacije.

![Zdravniki v aplikaciji eZdravje](https://github.com/horvatz/eZdravje/blob/master/img/eZdravje_doctors.PNG)

## Mobilna aplikacija :iphone:
V sklopu storitve eZdravje sva ustvarila tudi Android aplikacijo. Aplikacija pridobiva podatke preko **REST API-ja**.

Android aplikacija je namenjena zdravnikom. Omogoča **pregledovanje** in **dodajanje pacientov**.

Omogoča tudi prijavo zdravnika v aplikacijo.

<img align="left" src="https://github.com/horvatz/eZdravje/blob/master/img/eZdravje_mobile_login.png">


![Prijava v mobilno aplikacijo eZdravje](https://github.com/horvatz/eZdravje/blob/master/img/eZdravje_mobile_patients.png)

## Podatkovna baza :file_folder:
Aplikacija ima podatke shranjene v SQL podatkovni bazi. Bazaje gostovana na Microsoft Azure strežnikih.

V podatkovni bazi je 7 tabel, ki so del **ASP Identity** paketa in se uporabljajo za prijavo in registracijo na spletno aplikacijo.

V podatkovni bazi je še **6 tabel**:
* ***Patients*** - v tej tabeli so shranjeni vsi pacienti, vsak pacient je povezan tudi s svojim zdravnikom preko polja "SpecialistId"
* ***Prescriptions*** - v tej tabeli so shranjeni recepti ki so vezani na pacienta in jih je predpisal zdravnik
* ***Referrals*** - shranjene napotnice, ki so jih predpisali zdravniki za določenega pacienta
* ***Specialists*** - shranjeni vsi zdravniki, vsak zdravnik je tudi specialist iz področja definiranega v tablei "SpecialistCategories"
* ***SpecialistCategories*** - specializacije zdravnikov, vsak zdravnik ima eno
* ***ActivationCodes*** - aktivacijske kode za registracijo uporabnikov na storitev eZdraje

Spodaj je še **slika podatkovnega modela**: (Iz paketa Identity je vključena le tabela AspNetUsers)


![Slika podatkovnega modela](https://github.com/horvatz/eZdravje/blob/master/img/Database_diagram.PNG)
