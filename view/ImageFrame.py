import ttkbootstrap as ttk
from PIL import Image, ImageTk

class ImageFrame(ttk.Frame):
    def __init__(self, master, image, *args, **kwargs):
        super().__init__(master, *args, **kwargs)

        # Open and scale the image
        original_image = Image.open(image)
        scaled_image = original_image.resize((250, 250), Image.LANCZOS)  # Scale to 200x200 pixels
        self.image = ImageTk.PhotoImage(scaled_image)
        ttk.Label(self, image=self.image).pack()