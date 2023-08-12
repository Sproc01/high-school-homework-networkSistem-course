;**************************************************
;*** Struttura base programma con la PIC 16F84 ***
;***                                            ***
;    [Programma assoluto]
;
; (c) 2018, Michele Sprocatti
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
LED_ON  EQU     01					  ; Led acceso
LED_OFF EQU	    00					  ; Led spento
;=============================================================
;       AREA DATI
;=============================================================	
;LABEL	CODE 	OPERANDO	COMMENTO
;=============================================================
delayA EQU     0x0C					  
delayB EQU     0x0D
delayC EQU 	   0x0E	
cont   EQU     0x0F;variabile per controllo ripetizione ciclo	
ciclo  EQU     0xA0;variabile ripetizione ciclo somme nel ritorno
sottra EQU	   0xB0;variabile utilizzata per il ritorno
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
									;porta b in output
		Bsf STATUS,RP0
		clrf TRISB
		bcf STATUS, RP0
LOOP
		movlw 7
		movwf cont					;contatore per i cicli
		movlw 1
		movwf PORTB
	Andata
		call Delay					;routine di ritardo
		movf PORTB,0
		addwf PORTB					;spsostamento led tramite somme successive
		decfsz cont,1				;controllo ripetizione ciclo andata
	goto Andata
		movlw 7
		movwf cont					;contatore per i cicli
	Ritorno
		call Delay					;routine di ritardo
		movlw 1
		movwf sottra
	    movf cont,0
		movwf ciclo					;sistemazione valori variabili
		decf ciclo,1
			Somme					;ciclo con somme successive
				movf sottra,0
				addwf sottra,1
				decfsz ciclo,1		;controllo ripetizione ciclo somme
			goto Somme
		movf sottra,0
		subwf PORTB,1				;sottrazione per dimezzare il valore della porta
		decfsz cont,1				;controllo ripetizione ciclo ritorno
	goto Ritorno
goto LOOP							;ripetizione ciclo completo
;=============================================================
;       AREA ROUTINE
;=============================================================
;	
		Delay
		clrf delayA
		movlw 130
		movwf delayB
		movlw 5
		movwf delayC
	loop
		decfsz delayA
		goto loop
		decfsz delayB
		goto loop
		movlw 130
		movwf delayB
		decfsz delayC
	goto loop
 		return
											
        END                           ; Fine programma


 