   .model small
Org 100h 
MOV bp, 0400h   ;initialize data memory location 
MOV di, 0000h   ;set offset to zero              

LOOPIN: 
MOV ah, 01h     ;load keypress and store to al
 
INT 21h   

CMP al,40h   

JE Finish
Skip:
     ;
mov bh,al      ;store input while we print '='   


mov ah, 02h
       ;keypress interrupt     
mov DL,'='
Int 21h 
mov al,bh  ;mov input bach to al
mov bh,16  ;divide by 16 to and get remainder to convert from 1 value to 2 digits of hex
div bh
ADD AX,03010h   ;add 30 to get relative ascii value
mov cx,ax  
mov dh,39h      ; if either number  is > 39h add 7 to skip to A-F
cmp    dh,ch       
 jl     Less  
   
 jmp    Both            
Less: 
 ADD    ch,7   
Both:
 cmp    dh,cl       
 jl     Less2  
   
 jmp    Both2            
Less2: 
 ADD    ch,7   
Both2:



mov ah, 02h
mov dl,cl
int 21h 
mov dl, ch 
int 21h   
 
Call enter 
 
Call loopin     ;junk if nonzero 
  

DONE: 
RET 


ENTER:     ;prints a new line
MOV ah,02h 
MOV dl,0Dh 
INT 21h      
mov DL,0Ah
Int 21h 

RET          ;other half of call, returns to next line after call 

  


FINISH: 
