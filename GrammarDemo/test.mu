n = 9;
log "Print Odd and Even";

def list()
  return [7,8,9]
end

while n > 0 {

  # expressions can be surrounded by parenthesis, of course
  if (n % 2 == 0) {
    log n + " -> even";
  }
  else {
    log n + " -> odd";
  }

  n = n - 1;
}
  a=2;
  b=2;
  c= a+b;
  log "sum of a and b " + c;