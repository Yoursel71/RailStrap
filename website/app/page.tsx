import { Nav } from "@/components/nav";
import { Hero } from "@/components/hero";
import { Features } from "@/components/features";
import { PlaytimeFeature } from "@/components/playtime-feature";
import { DownloadSteps } from "@/components/download-steps";
import { Privacy } from "@/components/privacy";
import { FAQ } from "@/components/faq";
import { Footer } from "@/components/footer";
import { getAppVersion } from "@/lib/version";

export default function Home() {
  const version = getAppVersion();

  return (
    <>
      <Nav />
      <Hero version={version} />
      <Features />
      <PlaytimeFeature />
      <DownloadSteps />
      <Privacy />
      <FAQ />
      <Footer />
    </>
  );
}
