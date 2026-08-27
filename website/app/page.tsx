import { Nav } from "@/components/nav";
import { Hero } from "@/components/hero";
import { Features } from "@/components/features";
import { PlaytimeFeature } from "@/components/playtime-feature";
import { DownloadSteps } from "@/components/download-steps";
import { Privacy } from "@/components/privacy";
import { FAQ } from "@/components/faq";
import { Footer } from "@/components/footer";

export default function Home() {
  return (
    <>
      <Nav />
      <Hero />
      <Features />
      <PlaytimeFeature />
      <DownloadSteps />
      <Privacy />
      <FAQ />
      <Footer />
    </>
  );
}
