import cv2
from cvzone.HandTrackingModule import HandDetector
import socket

# Parameters
width, height = 1280, 720
display_scale = 0.3  # (0.5 = 50% size, 0.3 = 30% size)

# Webcam
cap = cv2.VideoCapture(0)
cap.set(3, width)
cap.set(4, height)

# Hand Detector
detector = HandDetector(maxHands=1, detectionCon=0.8)

# Communication
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 5052)

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
        
        # Send raw coordinates
        for lm in lmList:
            data.extend([lm[0], lm[1], lm[2]])
        
        # Send data to Unity
        sock.sendto(str.encode(str(data)), serverAddressPort)

    # --- NEW DISPLAY LOGIC ---
    # Create a smaller image just for display
    display_width = int(width * display_scale)
    display_height = int(height * display_scale)
    img_display = cv2.resize(img, (display_width, display_height))

    # Show the *resized* webcam feed
    cv2.imshow("Hand Tracking - Talk to the Hand", img_display)
    
    # Press 'q' to quit
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# Cleanup
cap.release()
cv2.destroyAllWindows()