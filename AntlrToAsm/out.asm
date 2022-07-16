org 100h
push offset x
mov ax, 8
push ax
pop ax
pop bx
mov [bx],ax
push offset r
mov ax, 1
push ax
pop ax
pop bx
mov [bx],ax
whiletestlbl0:
nop
push [x]
pop ax
cmp ax,0
jz whilelbl0
push offset r
push [r]
push [x]
pop ax
pop bx
mul bx
push ax
pop ax
pop bx
mov [bx],ax
push offset x
push [x]
mov ax, 1
push ax
pop bx
pop ax
sub ax,bx
push ax
pop ax
pop bx
mov [bx],ax
jmp whiletestlbl0
whilelbl0:
nop
push [r]
pop ax
call myprint
;dos terminate
mov ah,0x4C
int 0x21

myprint proc 
 cmp ax, 0
 jge poz 
 not ax
 add ax,1
 push ax
 mov dl, '-' ; print '-'
 mov ah, 02h
 int 21h   
 pop ax
poz:
 mov si,10d
 xor dx,dx
 push 10 ; newline
 mov cx,1d
nonzero:
 div si
 add dx,48d
 push dx
 inc cx
 xor dx,dx
 cmp ax,0h
 jne nonzero
writeloop:
 pop dx
 mov ah,02h
 int 21h
 dec cx
 jnz writeloop 
 mov dl, 13 ; carriage return
 mov ah, 02h ; 
 int 21h ;  
ret
endp
handlediv proc
 cmp ax,0
 jg axpos
 xor dx, dx
 cwd
 idiv bx
 ret
axpos:
 cmp bx,0
 jge bxpos
 xor dx, dx
 idiv bx
 ret
bxpos:
 xor dx, dx
 div bx
 ret
