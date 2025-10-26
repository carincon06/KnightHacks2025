import cv2
from cvzone.HandTrackingModule import HandDetector
import socket

# Parameters - SMALLER WINDOW SIZE
width, height = 640, 480  # Reduced from 1280x720 for smaller corner window

# Webcam
cap = cv2.VideoCapture(0)
cap.set(3, width)
cap.set(4, height)

# Hand Detector
detector = HandDetector(maxHands=1, detectionCon=0.8)

# Communication
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 5052)

# Create named window and position it in corner
cv2.namedWindow("Hand Tracking", cv2.WINDOW_NORMAL)
cv2.resizeWindow("Hand Tracking", 320, 240)  # Even smaller display window
cv2.moveWindow("Hand Tracking", 10, 10)  # Top-left corner

while True:
    # Get the frame from the webcam
    success, img = cap.read()
    
    if not success:
        print("Failed to grab frame")
        break
    
    # Hands
    hands, img = detector.findHands(img)

    data = []

    # Landmark values - (x,y,z) * 21
    if hands:
        # Get the first hand detected
        hand = hands[0]
        # Get the landmark list
        lmList = hand['lmList']
        
        for lm in lmList:
            data.extend([lm[0], height - lm[1], lm[2]])
        
        # Send data to Unity
        sock.sendto(str.encode(str(data)), serverAddressPort)

    # Show small window
    cv2.imshow("Hand Tracking", img)
    
    # Press 'q' to quit
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# Cleanup
cap.release()
cv2.destroyAllWindows()