void setup() {
  pinMode(2, INPUT);
  pinMode(3, INPUT);
  Serial.begin(57600);
}

void draw() {
  if (digitalRead(2) == 1) {
    Serial.write(0x00);
  }
  
if (digitalRead(3) == 1) {
    Serial.write(0x01);
  }
}
