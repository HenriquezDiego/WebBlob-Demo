namespace WebBlob_Demo.Models
{
    public class Image
    {
        public string Nombre { get; set; }
        public string UrlImg { get; set; }

        public Image(string nombre, string urlimg)
        {
            this.Nombre = nombre;
            this.UrlImg = urlimg;
        }
 
      
    }
}
