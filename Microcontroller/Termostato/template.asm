
;**************************************************
;*** Struttura base programma con la PIC 16F84 ***
;***                                            ***
;    [Programma assoluto]
;
; (c) 2015, Federico Melon
;
;**************************************************
        PROCESSOR       16F84A	     ;definizione del tipo di Pic per il quale è stato scritto il programma
        RADIX           DEC	         ;i numeri scritti senza notazione sono da intendersi come decimali
        INCLUDE         "P16F84A.INC" ;inclusione del file che contiene la definizione delle costanti di riferimento al file dei
        							 ;registri (memoria Ram)
        ERRORLEVEL      -302		 ;permette di escludere alcuni errori di compilazione, la  segnalazione  302  ricorda  di 
                                     ;commutare il banco di memoria qualora si utilizzino registri che non stanno nel banco 0


        ;Setup of PIC configuration flags
        ;XT oscillator
        ;Disable watch dog timer
        ;Enable power up timer
        ;Disable code protect
;        __CONFIG        0x3FF1	      ; definizione del file di configurazione
		__CONFIG   _XT_OSC & _CP_OFF & _WDT_OFF &_PWRTE_ON

;=============================================================
;       DEFINE
;=============================================================
;Definizione comandi
;#define  Bank1	bsf     STATUS,RP0			  ; Attiva banco 1
;#define  Bank0 bcf     STATUS,RP0	          ; Attiva banco 0
;=============================================================
; 		SIMBOLI
;=============================================================
;LABEL	CODE 	OPERANDO	COMMENTO
;=============================================================
PULSANTESX EQU 3
PULSANTEDX EQU 4
PULSANTE EQU 2
DELAYA	   EQU 0X0C
DELAYB     EQU 0X0D
stato      EQU 0x0E
flag       EQU 0x0F
cursor     EQU 0x10
premuti    EQU 0x11
sposta 	   EQU 0x12
;=============================================================
;       AREA DATI
;=============================================================	
;LABEL	CODE 	OPERANDO	COMMENTO
;=============================================================	
;=============================================================
;       PROGRAMMA PRINCIPALE
;=============================================================
;LABEL	CODE 	OPERANDO	COMMENTO
;=============================================================        ;Reset Vector
        ;Start point at CPU reset
        ORG     0x0000				  ;	indirizzo di inizio programma
		goto	Main
;=============================================================
;       INTERRUPT AREA
;=============================================================
		ORG     0x0004				  ;	indirizzo inizio routine interrupt
;
;
		retfie						  ; ritorno programma principale
;=============================================================
;       AREA PROGRAMMA PRINCIPALE
;=============================================================
		Main

		BSF STATUS,RP0  ;BANK 1
		MOVLW b'11000'
		MOVWF TRISA		;SETTATI I BIT 3 E 4 DELLA PORTA A IN INPUT E CORRISPONDONO AGLI SWITCH SINISTRO E DESTRO
		CLRF TRISB      ;PORTA B IN OUTPUT
		BCF STATUS,RP0  ;SPOSTAMENTO SU BANK 0
		MOVLW 1 
        MOVWF stato     ;inizializzazione registro
		CLRF PORTA		;inizializzazione porta A
        CLRF flag
		clrf cursor		;inizializzazione registro
		BCF STATUS,C 	;CONTROLLO DEL CARRY

LOOP    	
		BTFSS PORTA,PULSANTESX ;controllo bottone premuto
		CALL PULSANTE1
		BTFSS PORTA,PULSANTEDX
		CALL PULSANTE2        
		movf stato,W
		iorwf cursor,w 	;output tramite or tra i due registri(cursore e stato)
        movwf PORTB
		GOTO LOOP

PULSANTE1 	
		CALL Delay
		BTFSC PORTA,PULSANTESX
		return
		BTFSS PORTA,PULSANTEDX
		CALL entrambi ;premuti entrambi i bottoni
		BTFSC PORTA,PULSANTEDX
		CALL RUOTASX ;premuto solo uno
		return

PULSANTE2
		CALL Delay
		BTFSC PORTA,PULSANTEDX
		return
		BTFSS PORTA,PULSANTESX
		CALL entrambi ;premuti entrambi i bottoni
		BTFSC PORTA,PULSANTESX
		CALL RUOTADX ;premuto solo uno
		return

RUOTASX
		BTFSC flag,0
		goto ruotacurssx
		BCF STATUS,C
		BTFSS stato,7
		RLF stato
loop	BTFSS PORTA,PULSANTESX ;controllo se bottone è mantenuto premuto
		goto loop
		return
ruotacurssx
		BCF STATUS,C
		BTFSS cursor,7
		RLF cursor
loop2	BTFSS PORTA,PULSANTESX ;controllo se bottone è mantenuto premuto
		goto loop2
		goto TestLampadina
		return

RUOTADX
		BTFSC flag,0
		goto ruotacursdx
		BCF STATUS,C
		BTFSS stato,0
		RRF stato
loop1	BTFSS PORTA,PULSANTEDX ;controllo se bottone è mantenuto premuto
		goto loop1
		return
ruotacursdx
		BCF STATUS,C
		BTFSS cursor,0
		RRF cursor
loop3	BTFSS PORTA,PULSANTEDX ;controllo se bottone è mantenuto premuto
		goto loop3
		goto TestLampadina
		return

TestLampadina	;test per vedere se led cursore è maggiore del led di stato
        MOVF stato,w
        SUBWF cursor,w
        BTFSC STATUS, C
		BTFSS STATUS, Z
        Call AccendiLampadina ;caso sia maggiore
        BTFSS STATUS,C
        CALL SpegniLampadina ;caso sia minore
        return

AccendiLampadina ;accensione lampadina
     BSF PORTA,1
	 BSF PORTA,2 
      return

SpegniLampadina ;spegnimento lampadina
     BCF PORTA,1
	 BCF PORTA,2
     return
entrambi
		btfsc premuti,0
		goto entrambi1
		btfsc premuti,1
		goto entrambi2
entrambi1 	;quando vengono premuti entrambi i bottoni
		MOVLW 1
		bcf premuti,1
		bsf premuti,0
		movwf flag
        movwf cursor
		return	
entrambi2 	;quando vengono premuti entrambi i bottoni
		bcf premuti,0
		bsf premuti,1
        movf cursor,w
		movwf stato
		clrf cursor
		bsf cursor,1
		return	
;=============================================================
;       AREA ROUTINE
;=============================================================
;
Delay
		MOVLW 80
		MOVWF DELAYB
LOOP3	MOVLW 255
		MOVWF DELAYA
LOOP2 	DECFSZ DELAYA
		GOTO LOOP2
		DECFSZ DELAYB
		GOTO LOOP3
		return
        END                           ; Fine programma


 