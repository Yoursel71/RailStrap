"use client";

import { useEffect, useState } from "react";
import Image from "next/image";
import { useReducedMotion } from "framer-motion";
import { ShieldCheck } from "lucide-react";
import { ContainerScroll } from "@/components/ui/container-scroll-animation";
import { siteAsset } from "@/lib/site";

const TYPEWRITER_WORDS = ["upgraded.", "polished.", "optimized.", "personalized."];

function TypewriterText() {
  const shouldReduceMotion = useReducedMotion();
  const [wordIndex, setWordIndex] = useState(0);
  const [displayedText, setDisplayedText] = useState(TYPEWRITER_WORDS[0]);
  const [isDeleting, setIsDeleting] = useState(false);

  useEffect(() => {
    if (shouldReduceMotion) {
      setWordIndex(0);
      setDisplayedText(TYPEWRITER_WORDS[0]);
      setIsDeleting(false);
      return;
    }

    const currentWord = TYPEWRITER_WORDS[wordIndex];
    let delay = isDeleting ? 45 : 85;

    if (!isDeleting && displayedText === currentWord) {
      delay = 1600;
    } else if (isDeleting && displayedText === "") {
      delay = 250;
    }

    const timeout = window.setTimeout(() => {
      if (!isDeleting && displayedText === currentWord) {
        setIsDeleting(true);
        return;
      }

      if (isDeleting && displayedText === "") {
        setWordIndex((currentIndex) => (currentIndex + 1) % TYPEWRITER_WORDS.length);
        setIsDeleting(false);
        return;
      }

      const nextLength = displayedText.length + (isDeleting ? -1 : 1);
      setDisplayedText(currentWord.slice(0, nextLength));
    }, delay);

    return () => window.clearTimeout(timeout);
  }, [displayedText, isDeleting, shouldReduceMotion, wordIndex]);

  return (
    <span className="inline-grid align-baseline">
      <span className="invisible col-start-1 row-start-1 whitespace-nowrap" aria-hidden="true">
        personalized.
      </span>
      <span className="col-start-1 row-start-1 whitespace-nowrap" aria-hidden="true">
        <span className="bg-gradient-to-br from-[#b49cff] via-[#846fff] to-[#4f9dff] bg-clip-text text-transparent">
          {displayedText}
        </span>
        <span className="ml-[0.08em] inline-block h-[0.82em] w-[0.055em] translate-y-[0.06em] rounded-full bg-[#846fff] align-baseline animate-pulse motion-reduce:hidden" />
      </span>
      <span className="sr-only">upgraded.</span>
    </span>
  );
}

export function Hero({ version }: { version: string }) {
  return (
    <section id="hero" className="relative overflow-hidden">
      <div className="pointer-events-none absolute -left-40 -top-56 h-[34rem] w-[34rem] rounded-full bg-[#6d3fff]/25 blur-[110px]" />
      <div className="pointer-events-none absolute -right-40 -top-32 h-[30rem] w-[30rem] rounded-full bg-[#3f7bff]/20 blur-[110px]" />

      <ContainerScroll
        titleComponent={
          <div className="flex flex-col items-center px-4">
            <span className="mb-6 inline-flex items-center gap-2 rounded-full border border-[#7c5cff]/30 bg-[#7c5cff]/10 py-1.5 pl-2 pr-3 text-xs font-semibold text-[#c3b6ff] shadow-[0_0_30px_rgba(124,92,255,.12)]">
              <span className="h-1.5 w-1.5 rounded-full bg-[#9c82ff] shadow-[0_0_0_3px_rgba(124,92,255,.25)]" />
              Version {version} · Free and open source
            </span>

            <h1 className="max-w-[14ch] text-5xl font-extrabold leading-[0.98] tracking-[-0.045em] text-foreground sm:text-6xl md:text-[76px]">
              Your Roblox launcher, {" "}
              <TypewriterText />
            </h1>

            <p className="mx-auto mt-6 max-w-[56ch] text-base leading-7 text-muted-foreground sm:text-lg">
              Launch. Tune. Track. Keep it local.
            </p>

            <p className="mt-4 flex items-center justify-center gap-1.5 text-[12.5px] text-zinc-500">
              <ShieldCheck className="h-3.5 w-3.5" />
              Windows 10/11 · .NET 6 · no account required
            </p>
          </div>
        }
      >
        <div className="relative h-full w-full">
          <Image
            src={siteAsset("/screenshots/launch-menu.png")}
            alt="RailStrap launch menu showing Roblox, Studio, settings, and help actions"
            fill
            sizes="(min-width: 1024px) 1000px, 90vw"
            className="object-cover"
            priority
            draggable={false}
          />
        </div>
      </ContainerScroll>
    </section>
  );
}
