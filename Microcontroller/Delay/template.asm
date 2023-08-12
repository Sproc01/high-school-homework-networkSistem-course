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
		Bsf STATUS,RP0
		clrf TRISB
		bcf STATUS, RP0
		clrf PORTB
		Loop
		call Delay;routine ritardo
		btfss PORTB,0
		movlw LED_ON
		btfsc PORTB,0
		movlw LED_OFF
		movwf PORTB
		goto Loop
		nop
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


 