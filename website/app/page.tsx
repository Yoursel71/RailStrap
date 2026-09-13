import { Nav } from "@/components/nav";
import { Hero } from "@/components/hero";
import { ArtworkBand } from "@/components/artwork-band";
import { DownloadCTA } from "@/components/download-cta";
import { FAQ } from "@/components/faq";
import { getAppVersion } from "@/lib/version";

export default function Home() {
  const version = getAppVersion();

  return (
    <>
      <Nav />
      <Hero version={version} />
      <ArtworkBand />
      <DownloadCTA version={version} />
      <FAQ />
    </>
  );
}
