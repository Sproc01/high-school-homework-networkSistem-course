# MPLAB IDE generated this makefile for use with GNU make.
# Project: template.mcp
# Date: Sat Nov 25 11:00:39 2017

AS = MPASMWIN.exe
CC = 
LD = mplink.exe
AR = mplib.exe
RM = rm

template.cof : template.o
	$(CC) /p16F84 "template.o" /z__MPLAB_BUILD=1 /o"template.cof" /M"template.map" /W /x

template.o : template.asm C:/Program\ Files\ (x86)/Microchip/MPASM\ Suite/P16F84A.INC
	$(AS) /q /p16F84 "template.asm" /l"template.lst" /e"template.err"

clean : 
	$(CC) "template.o" "template.hex" "template.err" "template.lst" "template.cof"

