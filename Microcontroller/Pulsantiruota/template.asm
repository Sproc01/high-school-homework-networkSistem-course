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
LED_ON  EQU     01					  ; Led acceso
LED_OFF EQU	    00					  ; Led spento
;=============================================================
;       AREA DATI
;=============================================================
;LABEL	CODE 	OPERANDO	COMMENTO
;=============================================================
CounterA EQU     0x0C					  
CounterB EQU     0x0D
led		 EQU	 0x0E	
DelayE   EQU     0x0F
DelayF	 EQU     0x10
DelayG   EQU     0x11			
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
;Codice Programma
;        Bank1						  ;	accedo al banco zero del file register per settare I/O porta A e B
        bsf     STATUS,RP0			  ; attiva banco 1
        movlw   B'00011000'
        movwf   TRISA 				  
        movlw   B'00000000'			  
        movwf   TRISB
;        Bank0						  ; rinposta l'accesso ai registri del banco 1
	    bcf     STATUS,RP0	          ; attiva banco 0
		bcf	    STATUS,C
		movlw   1
		movwf   PORTB
Loop	call    Delay
		btfsc   PORTB,7
		goto	Dx
		btfsc   PORTB,0
		goto    Sx
		btfss   PORTA,4
		goto	Dx
		btfss   PORTA,3
		goto    Sx
		goto 	Loop
Dx	 	btfss   PORTA,4
		rrf     PORTB
		call    Delay
Pa		btfss   PORTA,4
		goto 	Pa
		goto    Loop
Sx		btfss   PORTA,3
		rlf		PORTB
		call    Delay
Pb		btfss   PORTA,3
		goto 	Pb
		goto 	Loop
		

;=============================================================
;       AREA ROUTINE
Delay
		clrf	DelayE
		movlw  	13
		movwf	DelayF
		movlw	2
		movwf	DelayG
Loop1	decfsz	DelayE
		goto	Loop1
		decfsz	DelayF
		goto 	Loop1
		movlw	130
		movwf	DelayF
		decfsz	DelayG
		goto	Loop1
		return
;=============================================================
;										
        END                           ; Fine programma