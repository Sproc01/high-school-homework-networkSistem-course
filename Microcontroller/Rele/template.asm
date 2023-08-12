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


        ;SetDX of PIC configuration flags
        ;XT oscillator
        ;Disable watch dog timer
        ;Enable power DX timer
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
DX		EQU		3		;bottone 1			 
SX		EQU		4		;bottone 2
;=============================================================
;       AREA DATI
;=============================================================	
;LABEL	CODE 	OPERANDO	COMMENTO
;=============================================================
cont    EQU 0x0C										
DelayA	EQU	0x0D  
DelayB	EQU	0x0E
DelayC  EQU 0x12
DelayD  EQU 0x13
DelayE	EQU	0x0F
DelayF	EQU	0x10
DelayG	EQU	0x11
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
		ORG     0x0004				  ;	indirizzo inizio routine interrDXt
;
;
		retfie						  ; ritorno programma principale
;=============================================================
;       AREA PROGRAMMA PRINCIPALE
;=============================================================
		Main
;Codice Programma					  		
        bsf     STATUS,RP0			  		
        movlw   B'11000'
        movwf   TRISA 				  	
        clrf	TRISB  				  		
	    bcf     STATUS,RP0          		
		movlw	1
		movwf	PORTB				  		;accende un led
		clrf	PORTA				  		;spengne la lampadina
		bcf		STATUS,C
Loop
		btfss	PORTA,DX		;controlla bottone sù (quello a destra)
		goto	testPulsanteDestro 				;tastoSù
		btfss	PORTA,SX		;controlla bottone giù
		goto	testPulsanteSinistro 		;tastoGiù
		goto	Loop
	
testPulsanteDestro
		call	Delay
		btfsc   PORTA,SX
		goto	ledSu
		goto    Timer	

testPulsanteSinistro
		call	Delay
		btfsc 	PORTA,DX
		goto	ledGiu
		goto	Timer

ledSu   									;sposta il led in sù
		call	Delay
		btfss	PORTB,7
		rlf		PORTB
test1
		btfss 	PORTA,DX
		goto	test1
		goto	Loop

ledGiu										;sposta il led in giù
		call	Delay
		btfss	PORTB,0
		rrf		PORTB
test
		btfss 	PORTA,SX
		goto	test
		goto	Loop

;===============================
;           DELAY
;===============================
Timer		
		movlw	b'00110'
		movwf	PORTA
		btfsc	PORTB,0
		goto	Loop4
Loop5	
		call	delay
		rrf		PORTB
		btfss	PORTB,0
		goto 	Loop5
Loop4		
		call	delay
		clrf	PORTA
		clrf	PORTB
		call 	delay
		movlw	B'00000001'
		movwf	PORTB
		goto	Loop

Delay
		movlw	27
		movwf	DelayB
Loop3	
		movlw	255
		movwf	DelayA
Loop2	
		decfsz	DelayA
		goto	Loop2
		decfsz	DelayB
		goto	Loop3
		return
	delay
		movlw 2
		movwf DelayD
		clrf DelayA
		movlw 130
		movwf DelayB
		movlw 5
		movwf DelayC
loop
		decfsz DelayA
		goto loop
		decfsz DelayB
		goto loop
		movlw 130
		movwf DelayB
		decfsz DelayC
		goto loop
		movlw 5
		movwf DelayC
		decfsz DelayD
		goto loop
 		return
											
        END         

 